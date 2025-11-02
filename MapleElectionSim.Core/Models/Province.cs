using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a province or territory within the country.
/// </summary>
public class Province
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public int Population { get; set; }

    public double HistoricalTurnout { get; set; }

    public List<Riding> Ridings { get; set; } = new();

    public Dictionary<string, double> IssueSalience { get; set; } = new();
}
