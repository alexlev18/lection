using System.Threading.Tasks;
using MapleElectionSim.Core.Events;
using MapleElectionSim.Core.Services;
using MapleElectionSim.Core.Simulation;
using Xunit;

namespace MapleElectionSim.Tests.Simulation;

public class CampaignSimulationTests
{
    [Fact]
    public async Task AdvanceAsync_ProducesSeatProjections()
    {
        var simulation = new CampaignSimulation(
            new SamplePollingDataService(),
            new SampleDemographicsService(),
            new SampleHistoricalResultsService(),
            new SimulationEventBus());

        var state = await simulation.CreateInitialStateAsync();
        var result = await simulation.AdvanceAsync(state);

        Assert.NotEmpty(result.RidingSeatProjections);
        Assert.NotEmpty(result.NationalPopularVote);
    }
}
