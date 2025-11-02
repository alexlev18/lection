using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;

namespace MapleElectionSim.Core.Services;

/// <summary>
/// Provides synthetic polling data for demo purposes.
/// </summary>
public class SamplePollingDataService : IPollingDataService
{
    private readonly IReadOnlyList<PollingDataPoint> _national;
    private readonly IReadOnlyList<PollingDataPoint> _province;
    private readonly IReadOnlyList<PollingDataPoint> _riding;

    public SamplePollingDataService()
    {
        var baseDate = new DateTime(2025, 8, 1);
        _national = Enumerable.Range(0, 10).Select(i => new PollingDataPoint
        {
            Date = baseDate.AddDays(i * 3),
            RegionId = "canada",
            PartySupport = new Dictionary<string, double>
            {
                ["lib"] = 0.34 + i * 0.002,
                ["cpc"] = 0.33 - i * 0.001,
                ["ndp"] = 0.19 + Math.Sin(i) * 0.01,
                ["gp"] = 0.08,
                ["bq"] = 0.06
            }
        }).ToList();

        _province = _national.Select(p => new PollingDataPoint
        {
            Date = p.Date,
            RegionId = "on",
            PartySupport = new Dictionary<string, double>
            {
                ["lib"] = p.PartySupport["lib"] + 0.02,
                ["cpc"] = p.PartySupport["cpc"] - 0.015,
                ["ndp"] = p.PartySupport["ndp"] + 0.01
            }
        }).ToList();

        _riding = _national.Select(p => new PollingDataPoint
        {
            Date = p.Date,
            RegionId = "on-001",
            PartySupport = new Dictionary<string, double>
            {
                ["lib"] = p.PartySupport["lib"] + 0.05,
                ["cpc"] = p.PartySupport["cpc"] - 0.03,
                ["ndp"] = p.PartySupport["ndp"] + 0.02
            }
        }).ToList();
    }

    public Task<IReadOnlyList<PollingDataPoint>> GetNationalPollingAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_national);

    public Task<IReadOnlyList<PollingDataPoint>> GetProvincePollingAsync(string provinceId, CancellationToken cancellationToken = default)
        => Task.FromResult(_province.Where(p => p.RegionId == provinceId).ToList() as IReadOnlyList<PollingDataPoint> ?? new List<PollingDataPoint>());

    public Task<IReadOnlyList<PollingDataPoint>> GetRidingPollingAsync(string ridingId, CancellationToken cancellationToken = default)
        => Task.FromResult(_riding.Where(p => p.RegionId == ridingId).ToList() as IReadOnlyList<PollingDataPoint> ?? new List<PollingDataPoint>());
}
