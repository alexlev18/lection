using System;
using System.Collections.Generic;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Simulation;

/// <summary>
/// Represents the mutable state tracked by the simulation engine.
/// </summary>
public class SimulationState
{
    public DateTime CurrentDate { get; set; }

    public Nation Nation { get; set; } = new()
    {
        Id = "canada",
        Name = "Canada"
    };

    public List<Party> Parties { get; set; } = new();

    public List<Candidate> Candidates { get; set; } = new();

    public List<CampaignEvent> Events { get; set; } = new();

    public SimulationParameters Parameters { get; set; } = new();

    public Dictionary<string, double> EconomicIndicators { get; set; } = new();

    public Dictionary<string, double> NationalMetrics { get; set; } = new();
}
