using System;
using System.Threading.Tasks;

namespace MapleElectionSim.Core.Events;

public interface ISimulationEventBus
{
    IDisposable Subscribe(Func<SimulationEvent, Task> handler);

    Task PublishAsync(SimulationEvent @event);
}
