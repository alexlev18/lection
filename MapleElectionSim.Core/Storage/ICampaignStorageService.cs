using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.Core.Storage;

public interface ICampaignStorageService
{
    Task SaveAsync(string path, SimulationState state, CancellationToken cancellationToken = default);

    Task<SimulationState?> LoadAsync(string path, CancellationToken cancellationToken = default);
}
