using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MapleElectionSim.Core.Events;

/// <summary>
/// Simple in-memory event aggregator supporting async handlers.
/// </summary>
public class SimulationEventBus : ISimulationEventBus
{
    private readonly ConcurrentDictionary<Guid, Func<SimulationEvent, Task>> _subscribers = new();

    public IDisposable Subscribe(Func<SimulationEvent, Task> handler)
    {
        var id = Guid.NewGuid();
        _subscribers[id] = handler;
        return new Subscription(_subscribers, id);
    }

    public async Task PublishAsync(SimulationEvent @event)
    {
        foreach (var subscriber in _subscribers.Values)
        {
            await subscriber(@event).ConfigureAwait(false);
        }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly ConcurrentDictionary<Guid, Func<SimulationEvent, Task>> _source;
        private readonly Guid _id;
        private bool _disposed;

        public Subscription(ConcurrentDictionary<Guid, Func<SimulationEvent, Task>> source, Guid id)
        {
            _source = source;
            _id = id;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _source.TryRemove(_id, out _);
            _disposed = true;
        }
    }
}
