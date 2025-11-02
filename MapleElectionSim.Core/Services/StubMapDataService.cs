using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

/// <summary>
/// Provides demo map geometry for the UI until real shapefiles are added.
/// </summary>
public class StubMapDataService : IMapDataService
{
    private readonly IReadOnlyList<MapRegionShape> _nationShapes;
    private readonly IReadOnlyList<MapRegionShape> _provinceShapes;
    private readonly IReadOnlyList<MapRegionShape> _ridingShapes;

    public StubMapDataService()
    {
        _nationShapes = new List<MapRegionShape>
        {
            new()
            {
                Id = "canada",
                Name = "Canada",
                Points = new()
                {
                    (60, -140), (50, -95), (45, -60), (55, -120)
                }
            }
        };

        _provinceShapes = new List<MapRegionShape>
        {
            new()
            {
                Id = "on",
                ParentId = "canada",
                Name = "Ontario",
                Points = new() { (55, -95), (42, -95), (42, -75), (55, -75) }
            },
            new()
            {
                Id = "qc",
                ParentId = "canada",
                Name = "Quebec",
                Points = new() { (55, -75), (46, -75), (46, -60), (55, -60) }
            }
        };

        _ridingShapes = new List<MapRegionShape>
        {
            new()
            {
                Id = "on-001",
                ParentId = "on",
                Name = "Ottawa Centre",
                Points = new() { (45.5, -76), (45.3, -76), (45.3, -75.5), (45.5, -75.5) }
            },
            new()
            {
                Id = "on-002",
                ParentId = "on",
                Name = "Toronto Lakeshore",
                Points = new() { (43.7, -79.6), (43.5, -79.6), (43.5, -79.2), (43.7, -79.2) }
            },
            new()
            {
                Id = "qc-001",
                ParentId = "qc",
                Name = "Montreal Centre",
                Points = new() { (45.6, -73.7), (45.4, -73.7), (45.4, -73.4), (45.6, -73.4) }
            }
        };
    }

    public Task<IReadOnlyList<MapRegionShape>> LoadNationShapesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_nationShapes);

    public Task<IReadOnlyList<MapRegionShape>> LoadProvinceShapesAsync(string provinceId, CancellationToken cancellationToken = default)
        => Task.FromResult(_provinceShapes.Where(p => p.ParentId == provinceId || provinceId == "canada").ToList() as IReadOnlyList<MapRegionShape> ?? new List<MapRegionShape>());

    public Task<IReadOnlyList<MapRegionShape>> LoadRidingShapesAsync(string provinceId, CancellationToken cancellationToken = default)
        => Task.FromResult(_ridingShapes.Where(r => r.ParentId == provinceId).ToList() as IReadOnlyList<MapRegionShape> ?? new List<MapRegionShape>());
}
