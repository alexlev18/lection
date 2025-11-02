using System;
using System.Threading.Tasks;
using System.Windows;
using MapleElectionSim.Core.Data;
using MapleElectionSim.Core.Events;
using MapleElectionSim.Core.Services;
using MapleElectionSim.Core.Simulation;
using MapleElectionSim.Core.Storage;
using MapleElectionSim.App.ViewModels;
using MapleElectionSim.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MapleElectionSim.App;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();

        _host.Start();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISimulationEventBus, SimulationEventBus>();
        services.AddSingleton<IPollingDataService, SamplePollingDataService>();
        services.AddSingleton<IDemographicsService, SampleDemographicsService>();
        services.AddSingleton<IHistoricalResultsService, SampleHistoricalResultsService>();
        services.AddSingleton<IMapDataService, StubMapDataService>();
        services.AddSingleton<ICampaignStorageService, JsonCampaignStorageService>();
        services.AddSingleton<CampaignSimulation>();
        services.AddSingleton<SampleDataSeeder>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MapViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<ProvinceViewModel>();
        services.AddSingleton<RidingViewModel>();
        services.AddSingleton<SettingsViewModel>();

        services.AddSingleton<MainWindow>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(3));
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
