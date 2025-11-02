using System.Collections.ObjectModel;
using System.Linq;
using MapleElectionSim.Core.Models;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Provides drill-down information for a selected province.
/// </summary>
public class ProvinceViewModel : ObservableObject
{
    private SimulationState? _state;
    private MapRegionShape? _selectedProvinceShape;
    private Province? _selectedProvince;

    public ProvinceViewModel()
    {
        RidingSummaries = new ObservableCollection<RidingSummary>();
    }

    public ObservableCollection<RidingSummary> RidingSummaries { get; }

    public MapRegionShape? SelectedProvinceShape
    {
        get => _selectedProvinceShape;
        set
        {
            if (SetProperty(ref _selectedProvinceShape, value) && value != null)
            {
                UpdateProvince(value.Id);
            }
        }
    }

    public Province? SelectedProvince
    {
        get => _selectedProvince;
        private set => SetProperty(ref _selectedProvince, value);
    }

    public void AttachState(SimulationState state)
    {
        _state = state;
    }

    public void UpdateFromResult(SimulationTickResult result)
    {
        if (SelectedProvince is null)
        {
            return;
        }

        PopulateRidingSummaries(SelectedProvince);
    }

    private void UpdateProvince(string provinceId)
    {
        if (_state is null)
        {
            return;
        }

        SelectedProvince = _state.Nation.Provinces.FirstOrDefault(p => p.Id == provinceId);
        if (SelectedProvince != null)
        {
            PopulateRidingSummaries(SelectedProvince);
        }
        else
        {
            RidingSummaries.Clear();
        }
    }

    private void PopulateRidingSummaries(Province province)
    {
        RidingSummaries.Clear();
        foreach (var riding in province.Ridings)
        {
            var leader = riding.PartySupport.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            var party = string.IsNullOrWhiteSpace(leader.Key) ? "N/A" : leader.Key;
            RidingSummaries.Add(new RidingSummary(riding.Name, party, leader.Value));
        }
    }

    public record RidingSummary(string RidingName, string? LeadingPartyId, double Support);
}
