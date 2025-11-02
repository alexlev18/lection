using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents simple polygon geometry metadata for the UI map layer.
/// </summary>
public class MapRegionShape
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public List<(double Latitude, double Longitude)> Points { get; set; } = new();

    public string? ParentId { get; set; }

    public string Type { get; set; } = "Polygon";
}
