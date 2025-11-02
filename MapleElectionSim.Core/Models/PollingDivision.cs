using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents the smallest geography we track for the simulation.
/// </summary>
public class PollingDivision
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public int Population { get; set; }

    public double HistoricalTurnout { get; set; }

    public List<DemographicGroup> Demographics { get; set; } = new();
}
