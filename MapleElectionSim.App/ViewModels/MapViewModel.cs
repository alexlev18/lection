using System.Collections.ObjectModel;
using System.Linq;
using MapleElectionSim.Core.Models;
using MapleElectionSim.Core.Services;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// View-model providing map geometry and selection state to the UI.
/// </summary>
public class MapViewModel : ObservableObject
{
    private readonly IMapDataService _mapDataService;
    private SimulationState? _state;
    private MapRegionShape? _selectedProvince;
    private MapRegionShape? _selectedRiding;

    public MapViewModel(IMapDataService mapDataService)
    {
        _mapDataService = mapDataService;
        Provinces = new ObservableCollection<MapRegionShape>();
        Ridings = new ObservableCollection<MapRegionShape>();

        SelectProvinceCommand = new RelayCommand(p => SelectedProvince = p as MapRegionShape);
        SelectRidingCommand = new RelayCommand(r => SelectedRiding = r as MapRegionShape);
    }

    public ObservableCollection<MapRegionShape> Provinces { get; }

    public ObservableCollection<MapRegionShape> Ridings { get; }

    public MapRegionShape? SelectedProvince
    {
        get => _selectedProvince;
        set
        {
            if (value != null && _state != null)
            {
                UpdateRidingsForProvince(value.Id);
            }

            SetProperty(ref _selectedProvince, value);
        }
    }

    public MapRegionShape? SelectedRiding
    {
        get => _selectedRiding;
        set => SetProperty(ref _selectedRiding, value);
    }

    public RelayCommand SelectProvinceCommand { get; }

    public RelayCommand SelectRidingCommand { get; }

    public async void AttachState(SimulationState state)
    {
        _state = state;
        Provinces.Clear();
        foreach (var province in await _mapDataService.LoadProvinceShapesAsync("canada"))
        {
            Provinces.Add(province);
        }

        if (Provinces.FirstOrDefault() is { } first)
        {
            SelectedProvince = first;
        }
    }

    public void UpdateFromResult(SimulationTickResult result)
    {
        // Could be used to update heatmaps; for now no-op.
    }

    private async void UpdateRidingsForProvince(string provinceId)
    {
        Ridings.Clear();
        foreach (var riding in await _mapDataService.LoadRidingShapesAsync(provinceId))
        {
            Ridings.Add(riding);
        }

        if (Ridings.FirstOrDefault() is { } first)
        {
            SelectedRiding = first;
        }
    }
}
