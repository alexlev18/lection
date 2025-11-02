using System;

namespace MapleElectionSim.Core.Events;

/// <summary>
/// Event payload broadcast when the simulation produces updates.
/// </summary>
public class SimulationEvent
{
    public SimulationEvent(string type, object? payload, DateTime timestamp)
    {
        Type = type;
        Payload = payload;
        Timestamp = timestamp;
    }

    public string Type { get; }

    public object? Payload { get; }

    public DateTime Timestamp { get; }
}
