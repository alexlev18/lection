namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents an issue being debated in the campaign.
/// </summary>
public class Issue
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public string Description { get; set; } = string.Empty;
}
