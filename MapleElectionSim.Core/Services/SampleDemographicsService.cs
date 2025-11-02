using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

/// <summary>
/// Provides demo demographic breakdowns.
/// </summary>
public class SampleDemographicsService : IDemographicsService
{
    private static readonly IReadOnlyList<DemographicGroup> BaseGroups = new List<DemographicGroup>
    {
        new()
        {
            Name = "Urban Professionals",
            PopulationShare = 0.35,
            HistoricalTurnout = 0.72,
            IssuePriorities = new()
            {
                ["climate"] = 0.8,
                ["economy"] = 0.6
            }
        },
        new()
        {
            Name = "Suburban Families",
            PopulationShare = 0.40,
            HistoricalTurnout = 0.68,
            IssuePriorities = new()
            {
                ["economy"] = 0.7,
                ["healthcare"] = 0.5
            }
        },
        new()
        {
            Name = "Rural Communities",
            PopulationShare = 0.25,
            HistoricalTurnout = 0.63,
            IssuePriorities = new()
            {
                ["agriculture"] = 0.7,
                ["economy"] = 0.5
            }
        }
    };

    public Task<IReadOnlyList<DemographicGroup>> GetNationalDemographicsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(BaseGroups);

    public Task<IReadOnlyList<DemographicGroup>> GetProvinceDemographicsAsync(string provinceId, CancellationToken cancellationToken = default)
        => Task.FromResult(BaseGroups.Select(Clone).ToList() as IReadOnlyList<DemographicGroup> ?? BaseGroups);

    public Task<IReadOnlyList<DemographicGroup>> GetRidingDemographicsAsync(string ridingId, CancellationToken cancellationToken = default)
        => Task.FromResult(BaseGroups.Select(Clone).ToList() as IReadOnlyList<DemographicGroup> ?? BaseGroups);

    private static DemographicGroup Clone(DemographicGroup group)
        => new()
        {
            Name = group.Name,
            PopulationShare = group.PopulationShare,
            HistoricalTurnout = group.HistoricalTurnout,
            IssuePriorities = new Dictionary<string, double>(group.IssuePriorities)
        };
}
