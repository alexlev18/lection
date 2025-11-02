using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

public interface IPollingDataService
{
    Task<IReadOnlyList<PollingDataPoint>> GetNationalPollingAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PollingDataPoint>> GetProvincePollingAsync(string provinceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PollingDataPoint>> GetRidingPollingAsync(string ridingId, CancellationToken cancellationToken = default);
}
