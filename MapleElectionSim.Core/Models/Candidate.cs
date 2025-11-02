namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents a candidate competing in a riding.
/// </summary>
public class Candidate
{
    public required string Id { get; init; }

    public required string PartyId { get; init; }

    public required string RidingId { get; init; }

    public required string Name { get; init; }

    public string Background { get; set; } = string.Empty;

    public double LocalFundraising { get; set; }
}
