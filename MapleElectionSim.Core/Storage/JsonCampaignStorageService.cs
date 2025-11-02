using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.Core.Storage;

/// <summary>
/// Persists simulation states to JSON files.
/// </summary>
public class JsonCampaignStorageService : ICampaignStorageService
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public async Task SaveAsync(string path, SimulationState state, CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, state, Options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SimulationState?> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await JsonSerializer.DeserializeAsync<SimulationState>(stream, Options, cancellationToken).ConfigureAwait(false);
    }
}
