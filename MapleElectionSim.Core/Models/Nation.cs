using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents the overall national view used for aggregations.
/// </summary>
public class Nation
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public List<Province> Provinces { get; set; } = new();

    public Dictionary<string, double> IssueSalience { get; set; } = new();
}
