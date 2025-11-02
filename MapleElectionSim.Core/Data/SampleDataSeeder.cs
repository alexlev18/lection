using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.Core.Data;

/// <summary>
/// Convenience helper to bootstrap the simulation with demo content.
/// </summary>
public class SampleDataSeeder
{
    private readonly CampaignSimulation _simulation;

    public SampleDataSeeder(CampaignSimulation simulation)
    {
        _simulation = simulation;
    }

    public Task<SimulationState> CreateAsync(CancellationToken cancellationToken = default)
        => _simulation.CreateInitialStateAsync(cancellationToken);
}
