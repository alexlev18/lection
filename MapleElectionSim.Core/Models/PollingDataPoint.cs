using System;
using System.Collections.Generic;

namespace MapleElectionSim.Core.Models;

/// <summary>
/// Represents polling trend data for a region and party.
/// </summary>
public class PollingDataPoint
{
    public DateTime Date { get; set; }

    public required string RegionId { get; init; }

    public Dictionary<string, double> PartySupport { get; set; } = new();
}
