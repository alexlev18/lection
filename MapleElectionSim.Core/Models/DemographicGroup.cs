using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a demographic cohort used when calculating riding level behaviour.
/// </summary>
public class DemographicGroup
{
    public required string Name { get; init; }

    /// <summary>
    /// Share of the riding population expressed as a value between 0 and 1.
    /// </summary>
    public double PopulationShare { get; set; }

    /// <summary>
    /// Historical turnout for the group expressed as a value between 0 and 1.
    /// </summary>
    public double HistoricalTurnout { get; set; }

    /// <summary>
    /// Policy priorities expressed as weights per issue identifier.
    /// </summary>
    public Dictionary<string, double> IssuePriorities { get; set; } = new();
}
