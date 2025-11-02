using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MapleElectionSim.Core.Services;

/// <summary>
/// Returns synthetic historical results for testing the projection calculations.
/// </summary>
public class SampleHistoricalResultsService : IHistoricalResultsService
{
    private static readonly IDictionary<string, double> Seats = new Dictionary<string, double>
    {
        ["lib"] = 157,
        ["cpc"] = 121,
        ["ndp"] = 24,
        ["bq"] = 32,
        ["gp"] = 3
    };

    public Task<IDictionary<string, double>> GetHistoricalSeatCountsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Seats);

    public Task<IDictionary<string, double>> GetHistoricalVoteShareAsync(string regionId, CancellationToken cancellationToken = default)
        => Task.FromResult<IDictionary<string, double>>(new Dictionary<string, double>
        {
            ["lib"] = 0.34,
            ["cpc"] = 0.33,
            ["ndp"] = 0.18,
            ["bq"] = regionId.StartsWith("qc") ? 0.28 : 0.02,
            ["gp"] = 0.07
        });
}
