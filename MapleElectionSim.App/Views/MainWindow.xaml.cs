using System.Windows;
using MapleElectionSim.App.ViewModels;

namespace MapleElectionSim.App.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        _viewModel.SettingsRequested += OnSettingsRequested;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Cleanup();
        _viewModel.SettingsRequested -= OnSettingsRequested;
    }

    private void OnSettingsRequested(object? sender, System.EventArgs e)
    {
        var dialog = new SettingsDialog
        {
            Owner = this,
            DataContext = _viewModel.Settings
        };

        dialog.ShowDialog();
    }
}
