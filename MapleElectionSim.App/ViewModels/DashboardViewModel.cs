using System.Collections.ObjectModel;
using System.Linq;
using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Provides national level metrics for display on the dashboard.
/// </summary>
public class DashboardViewModel : ObservableObject
{
    private SimulationState? _state;

    public DashboardViewModel()
    {
        SeatProjections = new ObservableCollection<KeyValuePair<string, double>>();
        PopularVote = new ObservableCollection<KeyValuePair<string, double>>();
    }

    public ObservableCollection<KeyValuePair<string, double>> SeatProjections { get; }

    public ObservableCollection<KeyValuePair<string, double>> PopularVote { get; }

    public double TotalFundraising => _state?.NationalMetrics.GetValueOrDefault("fundraising") ?? 0;

    public void AttachState(SimulationState state)
    {
        _state = state;
        RefreshFromState();
    }

    public void UpdateMetrics(SimulationTickResult result)
    {
        SeatProjections.Clear();
        foreach (var seat in result.RidingSeatProjections.OrderByDescending(kvp => kvp.Value))
        {
            SeatProjections.Add(seat);
        }

        PopularVote.Clear();
        foreach (var vote in result.NationalPopularVote.OrderByDescending(kvp => kvp.Value))
        {
            PopularVote.Add(vote);
        }

        RaisePropertyChanged(nameof(TotalFundraising));
    }

    private void RefreshFromState()
    {
        if (_state is null)
        {
            return;
        }

        SeatProjections.Clear();
        foreach (var party in _state.Parties)
        {
            SeatProjections.Add(new KeyValuePair<string, double>(party.Name, 0));
        }

        PopularVote.Clear();
        foreach (var party in _state.Parties)
        {
            PopularVote.Add(new KeyValuePair<string, double>(party.Name, party.BaseSupport));
        }

        RaisePropertyChanged(nameof(TotalFundraising));
    }
}
