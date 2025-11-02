using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

public interface IDemographicsService
{
    Task<IReadOnlyList<DemographicGroup>> GetNationalDemographicsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DemographicGroup>> GetProvinceDemographicsAsync(string provinceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DemographicGroup>> GetRidingDemographicsAsync(string ridingId, CancellationToken cancellationToken = default);
}
