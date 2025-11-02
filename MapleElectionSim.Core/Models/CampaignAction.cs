using System;
using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a scheduled action that a party intends to execute.
/// </summary>
public class CampaignAction
{
    public required string Id { get; init; }

    public required string PartyId { get; init; }

    public string? TargetProvinceId { get; set; }

    public string? TargetRidingId { get; set; }

    public ActionType Type { get; set; }

    public DateTime ScheduledDate { get; set; }

    public Dictionary<string, double> IssueEmphasis { get; set; } = new();

    public double Budget { get; set; }
}

public enum ActionType
{
    Rally,
    Advertising,
    Canvassing,
    DebatePreparation,
    MediaInterview
}
