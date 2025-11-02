using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

public interface IMapDataService
{
    Task<IReadOnlyList<MapRegionShape>> LoadNationShapesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MapRegionShape>> LoadProvinceShapesAsync(string provinceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MapRegionShape>> LoadRidingShapesAsync(string provinceId, CancellationToken cancellationToken = default);
}
