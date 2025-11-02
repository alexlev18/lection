using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MapleElectionSim.Core.Events;
using MapleElectionSim.Core.Simulation;
using MapleElectionSim.Core.Storage;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Root view-model coordinating the simulation controls and nested dashboards.
/// </summary>
public class MainViewModel : ObservableObject
{
    private readonly CampaignSimulation _simulation;
    private readonly SampleDataSeeder _dataSeeder;
    private readonly ISimulationEventBus _eventBus;
    private readonly ICampaignStorageService _storageService;
    private readonly MapViewModel _mapViewModel;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly ProvinceViewModel _provinceViewModel;
    private readonly RidingViewModel _ridingViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private IDisposable? _subscription;
    private SimulationState? _state;
    private bool _isRunning;
    private string _statusMessage = "Ready";
    private CancellationTokenSource? _loopCts;

    public MainViewModel(
        CampaignSimulation simulation,
        SampleDataSeeder dataSeeder,
        ISimulationEventBus eventBus,
        ICampaignStorageService storageService,
        MapViewModel mapViewModel,
        DashboardViewModel dashboardViewModel,
        ProvinceViewModel provinceViewModel,
        RidingViewModel ridingViewModel,
        SettingsViewModel settingsViewModel)
    {
        _simulation = simulation;
        _dataSeeder = dataSeeder;
        _eventBus = eventBus;
        _storageService = storageService;
        _mapViewModel = mapViewModel;
        _dashboardViewModel = dashboardViewModel;
        _provinceViewModel = provinceViewModel;
        _ridingViewModel = ridingViewModel;
        _settingsViewModel = settingsViewModel;
        _mapViewModel.PropertyChanged += OnMapSelectionChanged;

        TimelineMessages = new ObservableCollection<string>();

        StartCommand = new RelayCommand(async _ => await StartSimulationAsync());
        PauseCommand = new RelayCommand(_ => PauseSimulation(), _ => IsRunning);
        StepCommand = new RelayCommand(async _ => await StepSimulationAsync(), _ => !IsRunning);
        SaveCommand = new RelayCommand(async _ => await SaveAsync());
        LoadCommand = new RelayCommand(async _ => await LoadAsync());
        OpenSettingsCommand = new RelayCommand(_ => SettingsRequested?.Invoke(this, EventArgs.Empty));
    }

    public event EventHandler? SettingsRequested;

    public ObservableCollection<string> TimelineMessages { get; }

    public MapViewModel Map => _mapViewModel;

    public DashboardViewModel Dashboard => _dashboardViewModel;

    public ProvinceViewModel Province => _provinceViewModel;

    public RidingViewModel Riding => _ridingViewModel;

    public SettingsViewModel Settings => _settingsViewModel;

    public SimulationState? SimulationState
    {
        get => _state;
        private set => SetProperty(ref _state, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            SetProperty(ref _isRunning, value);
            PauseCommand.RaiseCanExecuteChanged();
            StepCommand.RaiseCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public RelayCommand StartCommand { get; }

    public RelayCommand PauseCommand { get; }

    public RelayCommand StepCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand LoadCommand { get; }

    public RelayCommand OpenSettingsCommand { get; }

    public async Task InitializeAsync()
    {
        SimulationState = await _dataSeeder.CreateAsync();
        _mapViewModel.AttachState(SimulationState);
        _dashboardViewModel.AttachState(SimulationState);
        _provinceViewModel.AttachState(SimulationState);
        _ridingViewModel.AttachState(SimulationState);
        _settingsViewModel.AttachParameters(SimulationState.Parameters);

        _subscription = _eventBus.Subscribe(async evt =>
        {
            if (evt.Payload is SimulationTickResult result)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => ApplyTickResult(result));
            }
        });

        StatusMessage = "Simulation initialised";
    }

    private async Task StartSimulationAsync()
    {
        if (SimulationState is null)
        {
            return;
        }

        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        _loopCts = new CancellationTokenSource();

        try
        {
            while (!_loopCts.IsCancellationRequested)
            {
                await _simulation.AdvanceAsync(SimulationState, _loopCts.Token).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(1), _loopCts.Token).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
            // Expected when paused.
        }
        finally
        {
            IsRunning = false;
        }
    }

    private void PauseSimulation()
    {
        _loopCts?.Cancel();
        StatusMessage = "Simulation paused";
    }

    private async Task StepSimulationAsync()
    {
        if (SimulationState is null)
        {
            return;
        }

        await _simulation.AdvanceAsync(SimulationState);
    }

    private async Task SaveAsync()
    {
        if (SimulationState is null)
        {
            return;
        }

        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MapleElectionSim.json");
        await _storageService.SaveAsync(path, SimulationState);
        StatusMessage = $"Saved to {path}";
    }

    private async Task LoadAsync()
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MapleElectionSim.json");
        var loaded = await _storageService.LoadAsync(path);
        if (loaded != null)
        {
            SimulationState = loaded;
            _mapViewModel.AttachState(SimulationState);
            _dashboardViewModel.AttachState(SimulationState);
            _provinceViewModel.AttachState(SimulationState);
            _ridingViewModel.AttachState(SimulationState);
            _settingsViewModel.AttachParameters(SimulationState.Parameters);
            StatusMessage = "Campaign loaded";
        }
        else
        {
            StatusMessage = "No saved campaign found";
        }
    }

    private void ApplyTickResult(SimulationTickResult result)
    {
        TimelineMessages.Add(string.Join(" - ", result.Date.ToShortDateString(), result.Messages.FirstOrDefault()));
        _dashboardViewModel.UpdateMetrics(result);
        _mapViewModel.UpdateFromResult(result);
        _provinceViewModel.UpdateFromResult(result);
        _ridingViewModel.UpdateFromResult(result);
        StatusMessage = "Simulation tick complete";
    }

    public void Cleanup()
    {
        _subscription?.Dispose();
        _loopCts?.Cancel();
        _mapViewModel.PropertyChanged -= OnMapSelectionChanged;
    }

    private void OnMapSelectionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapViewModel.SelectedProvince))
        {
            _provinceViewModel.SelectedProvinceShape = _mapViewModel.SelectedProvince;
        }

        if (e.PropertyName == nameof(MapViewModel.SelectedRiding))
        {
            _ridingViewModel.SelectedRidingShape = _mapViewModel.SelectedRiding;
        }
    }
}
