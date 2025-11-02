using System;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents an event influencing the simulation such as debates or scandals.
/// </summary>
public class CampaignEvent
{
    public required string Id { get; init; }

    public DateTime Date { get; set; }

    public required string Title { get; init; }

    public string Description { get; set; } = string.Empty;

    public double Impact { get; set; }

    public string? AffectedPartyId { get; set; }

    public string? AffectedRegionId { get; set; }
}
