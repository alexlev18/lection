using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a federal political party and the high-level campaign state.
/// </summary>
public class Party
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string Leader { get; set; } = string.Empty;

    public string ColourHex { get; set; } = "#888888";

    public double BaseSupport { get; set; }

    public Dictionary<string, double> IssuePositions { get; set; } = new();

    public double NationalFundraising { get; set; }

    public List<CampaignAction> ScheduledActions { get; set; } = new();
}
