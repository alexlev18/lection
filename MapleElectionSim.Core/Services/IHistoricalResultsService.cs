using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MapleElectionSim.Core.Services;

public interface IHistoricalResultsService
{
    Task<IDictionary<string, double>> GetHistoricalSeatCountsAsync(CancellationToken cancellationToken = default);

    Task<IDictionary<string, double>> GetHistoricalVoteShareAsync(string regionId, CancellationToken cancellationToken = default);
}
