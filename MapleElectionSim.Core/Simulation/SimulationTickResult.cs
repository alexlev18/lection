using System;
using System.Collections.Generic;

namespace MapleElectionSim.Core.Simulation;

/// <summary>
/// Result of advancing the simulation by one tick.
/// </summary>
public class SimulationTickResult
{
    public DateTime Date { get; set; }

    public Dictionary<string, double> RidingSeatProjections { get; set; } = new();

    public Dictionary<string, double> NationalPopularVote { get; set; } = new();

    public Dictionary<string, double> FundraisingChanges { get; set; } = new();

    public List<string> Messages { get; set; } = new();
}
