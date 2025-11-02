using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a federal riding.
/// </summary>
public class Riding
{
    public required string Id { get; init; }

    public required string ProvinceId { get; init; }

    public required string Name { get; init; }

    public int Population { get; set; }

    public double HistoricalTurnout { get; set; }

    public List<PollingDivision> PollingDivisions { get; set; } = new();

    public Dictionary<string, double> PartySupport { get; set; } = new();

    public Dictionary<string, double> IssueSalience { get; set; } = new();
}
