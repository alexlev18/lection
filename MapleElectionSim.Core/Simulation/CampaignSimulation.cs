using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Events;
using MapleElectionSim.Core.Models;
using MapleElectionSim.Core.Services;

namespace MapleElectionSim.Core.Simulation;

/// <summary>
/// Coordinates the simulation tick loop.
/// </summary>
public class CampaignSimulation
{
    private readonly IPollingDataService _pollingDataService;
    private readonly IDemographicsService _demographicsService;
    private readonly IHistoricalResultsService _historicalResultsService;
    private readonly ISimulationEventBus _eventBus;

    public CampaignSimulation(
        IPollingDataService pollingDataService,
        IDemographicsService demographicsService,
        IHistoricalResultsService historicalResultsService,
        ISimulationEventBus eventBus)
    {
        _pollingDataService = pollingDataService;
        _demographicsService = demographicsService;
        _historicalResultsService = historicalResultsService;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Creates a demo state that is ready to be shown in the UI.
    /// </summary>
    public async Task<SimulationState> CreateInitialStateAsync(CancellationToken cancellationToken = default)
    {
        var state = new SimulationState
        {
            CurrentDate = new DateTime(2025, 8, 1)
        };

        state.Parties.AddRange(CreateParties());
        state.Nation.Provinces.AddRange(CreateProvinces());
        state.Candidates.AddRange(CreateCandidates());
        state.EconomicIndicators["gdp"] = 1.5;
        state.EconomicIndicators["unemployment"] = 5.8;
        state.NationalMetrics["fundraising"] = state.Parties.Sum(p => p.NationalFundraising);

        var demographics = await _demographicsService.GetNationalDemographicsAsync(cancellationToken).ConfigureAwait(false);
        state.Nation.IssueSalience = CalculateNationalIssueSalience(demographics);

        var polling = await _pollingDataService.GetNationalPollingAsync(cancellationToken).ConfigureAwait(false);
        var latest = polling.OrderBy(p => p.Date).LastOrDefault();
        if (latest != null)
        {
            foreach (var party in state.Parties)
            {
                party.BaseSupport = latest.PartySupport.TryGetValue(party.Id, out var support) ? support : party.BaseSupport;
            }
        }

        foreach (var province in state.Nation.Provinces)
        {
            foreach (var riding in province.Ridings)
            {
                foreach (var party in state.Parties)
                {
                    var support = latest?.PartySupport.TryGetValue(party.Id, out var latestSupport) == true
                        ? latestSupport
                        : party.BaseSupport;
                    riding.PartySupport[party.Id] = support;
                }
            }
        }

        var historicalSeats = await _historicalResultsService.GetHistoricalSeatCountsAsync(cancellationToken).ConfigureAwait(false);
        foreach (var kvp in historicalSeats)
        {
            state.NationalMetrics[$"historical_{kvp.Key}_seats"] = kvp.Value;
        }

        return state;
    }

    public async Task<SimulationTickResult> AdvanceAsync(SimulationState state, CancellationToken cancellationToken = default)
    {
        var tickLength = state.Parameters.WeeklyTick ? 7 : 1;
        state.CurrentDate = state.CurrentDate.AddDays(tickLength);

        // Apply scheduled actions happening this tick.
        foreach (var party in state.Parties)
        {
            var actions = party.ScheduledActions
                .Where(a => a.ScheduledDate.Date <= state.CurrentDate.Date)
                .ToList();

            foreach (var action in actions)
            {
                ApplyActionToState(state, party, action);
                party.ScheduledActions.Remove(action);
            }
        }

        // Gradually decay enthusiasm.
        foreach (var party in state.Parties)
        {
            party.BaseSupport = Math.Clamp(party.BaseSupport - state.Parameters.EnthusiasmDecay * 0.01, 0.05, 0.9);
        }

        var projections = CalculateSeatProjections(state);
        var fundraising = CalculateFundraising(state);

        state.NationalMetrics["fundraising"] = fundraising.Sum(kvp => kvp.Value);

        var tickResult = new SimulationTickResult
        {
            Date = state.CurrentDate,
            NationalPopularVote = state.Parties.ToDictionary(p => p.Id, p => p.BaseSupport),
            RidingSeatProjections = projections,
            FundraisingChanges = fundraising,
            Messages = new List<string>
            {
                $"Simulation advanced to {state.CurrentDate:MMM dd}.",
                "Seat projections updated using weighted polling trends."
            }
        };

        await _eventBus.PublishAsync(new SimulationEvent("SimulationTick", tickResult, DateTime.UtcNow)).ConfigureAwait(false);

        return tickResult;
    }

    private static void ApplyActionToState(SimulationState state, Party party, CampaignAction action)
    {
        var boost = action.Type switch
        {
            ActionType.Rally => 0.012,
            ActionType.Advertising => 0.008,
            ActionType.Canvassing => 0.010,
            ActionType.DebatePreparation => 0.006,
            ActionType.MediaInterview => 0.005,
            _ => 0.004
        };

        if (!string.IsNullOrWhiteSpace(action.TargetRidingId))
        {
            var riding = state.Nation.Provinces.SelectMany(p => p.Ridings)
                .FirstOrDefault(r => r.Id == action.TargetRidingId);
            if (riding != null)
            {
                riding.PartySupport[party.Id] = riding.PartySupport.GetValueOrDefault(party.Id) + boost;
            }
        }
        else if (!string.IsNullOrWhiteSpace(action.TargetProvinceId))
        {
            foreach (var riding in state.Nation.Provinces.First(p => p.Id == action.TargetProvinceId).Ridings)
            {
                riding.PartySupport[party.Id] = riding.PartySupport.GetValueOrDefault(party.Id) + boost * 0.7;
            }
        }
        else
        {
            party.BaseSupport = Math.Clamp(party.BaseSupport + boost * 0.5, 0, 1);
        }

        party.NationalFundraising += action.Budget * state.Parameters.FundraisingMultiplier;
    }

    private IDictionary<string, double> CalculateSeatProjections(SimulationState state)
    {
        var totals = new Dictionary<string, double>();
        foreach (var riding in state.Nation.Provinces.SelectMany(p => p.Ridings))
        {
            if (!riding.PartySupport.Any())
            {
                foreach (var party in state.Parties)
                {
                    riding.PartySupport[party.Id] = party.BaseSupport;
                }
            }

            var winningParty = riding.PartySupport.MaxBy(kvp => kvp.Value);
            if (winningParty.Key != null)
            {
                totals[winningParty.Key] = totals.GetValueOrDefault(winningParty.Key) + 1;
            }
        }

        return totals;
    }

    private IDictionary<string, double> CalculateFundraising(SimulationState state)
    {
        var results = new Dictionary<string, double>();
        foreach (var party in state.Parties)
        {
            var amount = party.BaseSupport * 100000 * state.Parameters.FundraisingMultiplier;
            party.NationalFundraising += amount;
            results[party.Id] = amount;
        }

        return results;
    }

    private static Dictionary<string, double> CalculateNationalIssueSalience(IReadOnlyList<DemographicGroup> demographics)
    {
        var totals = new Dictionary<string, double>();
        foreach (var group in demographics)
        {
            foreach (var issue in group.IssuePriorities)
            {
                totals[issue.Key] = totals.GetValueOrDefault(issue.Key) + issue.Value * group.PopulationShare;
            }
        }

        return totals;
    }

    private static IEnumerable<Party> CreateParties()
    {
        yield return new Party
        {
            Id = "lib",
            Name = "Liberal Party",
            Leader = "Alex Laurent",
            ColourHex = "#D71920",
            BaseSupport = 0.33,
            NationalFundraising = 10_000_000
        };

        yield return new Party
        {
            Id = "cpc",
            Name = "Conservative Party",
            Leader = "Morgan Steele",
            ColourHex = "#0C499C",
            BaseSupport = 0.32,
            NationalFundraising = 9_500_000
        };

        yield return new Party
        {
            Id = "ndp",
            Name = "New Democratic Party",
            Leader = "Riley Singh",
            ColourHex = "#F58220",
            BaseSupport = 0.18,
            NationalFundraising = 4_200_000
        };

        yield return new Party
        {
            Id = "gp",
            Name = "Green Party",
            Leader = "Jordan Oak",
            ColourHex = "#3D9B35",
            BaseSupport = 0.07,
            NationalFundraising = 1_200_000
        };

        yield return new Party
        {
            Id = "bq",
            Name = "Bloc Québécois",
            Leader = "Camille Fortin",
            ColourHex = "#1A98D5",
            BaseSupport = 0.05,
            NationalFundraising = 1_600_000
        };
    }

    private static IEnumerable<Province> CreateProvinces()
    {
        yield return new Province
        {
            Id = "on",
            Name = "Ontario",
            Population = 14_000_000,
            HistoricalTurnout = 0.64,
            Ridings = new List<Riding>
            {
                new()
                {
                    Id = "on-001",
                    ProvinceId = "on",
                    Name = "Ottawa Centre",
                    Population = 120_000,
                    HistoricalTurnout = 0.68
                },
                new()
                {
                    Id = "on-002",
                    ProvinceId = "on",
                    Name = "Toronto Lakeshore",
                    Population = 135_000,
                    HistoricalTurnout = 0.66
                }
            }
        };

        yield return new Province
        {
            Id = "qc",
            Name = "Quebec",
            Population = 8_400_000,
            HistoricalTurnout = 0.62,
            Ridings = new List<Riding>
            {
                new()
                {
                    Id = "qc-001",
                    ProvinceId = "qc",
                    Name = "Montreal Centre",
                    Population = 110_000,
                    HistoricalTurnout = 0.65
                }
            }
        };
    }

    private static IEnumerable<Candidate> CreateCandidates()
    {
        yield return new Candidate
        {
            Id = "cand-1",
            Name = "Taylor Bennett",
            PartyId = "lib",
            RidingId = "on-001",
            Background = "Former city councillor"
        };
        yield return new Candidate
        {
            Id = "cand-2",
            Name = "Casey Morgan",
            PartyId = "cpc",
            RidingId = "on-001",
            Background = "Local entrepreneur"
        };
        yield return new Candidate
        {
            Id = "cand-3",
            Name = "Avery Choi",
            PartyId = "ndp",
            RidingId = "on-002",
            Background = "Community organizer"
        };
    }
}
