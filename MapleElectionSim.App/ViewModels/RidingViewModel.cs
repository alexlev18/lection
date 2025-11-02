using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MapleElectionSim.Core.Models;
using MapleElectionSim.Core.Services;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Provides riding-level drill-down information such as demographics and polling trends.
/// </summary>
public class RidingViewModel : ObservableObject
{
    private readonly IDemographicsService _demographicsService;
    private readonly IPollingDataService _pollingDataService;
    private SimulationState? _state;
    private MapRegionShape? _selectedRidingShape;
    private Riding? _selectedRiding;

    public RidingViewModel(IDemographicsService demographicsService, IPollingDataService pollingDataService)
    {
        _demographicsService = demographicsService;
        _pollingDataService = pollingDataService;

        Demographics = new ObservableCollection<KeyValuePair<string, double>>();
        PollingTrend = new ObservableCollection<KeyValuePair<string, double>>();
        PartySupport = new ObservableCollection<KeyValuePair<string, double>>();
    }

    public ObservableCollection<KeyValuePair<string, double>> Demographics { get; }

    public ObservableCollection<KeyValuePair<string, double>> PollingTrend { get; }

    public ObservableCollection<KeyValuePair<string, double>> PartySupport { get; }

    public MapRegionShape? SelectedRidingShape
    {
        get => _selectedRidingShape;
        set
        {
            if (SetProperty(ref _selectedRidingShape, value) && value != null)
            {
                _ = UpdateRidingAsync(value.Id);
            }
        }
    }

    public Riding? SelectedRiding
    {
        get => _selectedRiding;
        private set => SetProperty(ref _selectedRiding, value);
    }

    public void AttachState(SimulationState state)
    {
        _state = state;
    }

    public void UpdateFromResult(SimulationTickResult result)
    {
        if (SelectedRiding is null)
        {
            return;
        }

        UpdatePartySupport(SelectedRiding);
    }

    private async Task UpdateRidingAsync(string ridingId)
    {
        if (_state is null)
        {
            return;
        }

        var riding = _state.Nation.Provinces.SelectMany(p => p.Ridings).FirstOrDefault(r => r.Id == ridingId);
        if (riding is null)
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                SelectedRiding = null;
                Demographics.Clear();
                PollingTrend.Clear();
                PartySupport.Clear();
            });
            return;
        }

        var demographics = await _demographicsService.GetRidingDemographicsAsync(ridingId).ConfigureAwait(false);
        var polling = await _pollingDataService.GetRidingPollingAsync(ridingId).ConfigureAwait(false);

        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            SelectedRiding = riding;
            UpdatePartySupport(SelectedRiding);

            Demographics.Clear();
            foreach (var group in demographics)
            {
                Demographics.Add(new KeyValuePair<string, double>(group.Name, group.PopulationShare));
            }

            PollingTrend.Clear();
            var leadingParty = SelectedRiding.PartySupport.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
            foreach (var point in polling.OrderBy(p => p.Date))
            {
                var total = point.PartySupport.GetValueOrDefault(leadingParty);
                PollingTrend.Add(new KeyValuePair<string, double>(point.Date.ToString("MM/dd"), total));
            }
        });
    }

    private void UpdatePartySupport(Riding riding)
    {
        PartySupport.Clear();
        foreach (var support in riding.PartySupport.OrderByDescending(kvp => kvp.Value))
        {
            PartySupport.Add(new KeyValuePair<string, double>(support.Key, support.Value));
        }
    }
}
