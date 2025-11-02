using MapleElectionSim.Core.Simulation;

namespace MapleElectionSim.App.ViewModels;

/// <summary>
/// Provides user editable simulation parameters.
/// </summary>
public class SettingsViewModel : ObservableObject
{
    private SimulationParameters? _parameters;

    public SimulationParameters? Parameters
    {
        get => _parameters;
        private set
        {
            if (SetProperty(ref _parameters, value))
            {
                RaisePropertyChanged(nameof(EconomicGrowth));
                RaisePropertyChanged(nameof(CostOfLivingIndex));
                RaisePropertyChanged(nameof(MediaImpactWeight));
                RaisePropertyChanged(nameof(ScandalWeight));
                RaisePropertyChanged(nameof(PolicyAnnouncementBoost));
                RaisePropertyChanged(nameof(BaseTurnout));
                RaisePropertyChanged(nameof(WeeklyTick));
            }
        }
    }

    public double EconomicGrowth
    {
        get => Parameters?.EconomicGrowth ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.EconomicGrowth = value;
            RaisePropertyChanged();
        }
    }

    public double CostOfLivingIndex
    {
        get => Parameters?.CostOfLivingIndex ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.CostOfLivingIndex = value;
            RaisePropertyChanged();
        }
    }

    public double MediaImpactWeight
    {
        get => Parameters?.MediaImpactWeight ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.MediaImpactWeight = value;
            RaisePropertyChanged();
        }
    }

    public double ScandalWeight
    {
        get => Parameters?.ScandalWeight ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.ScandalWeight = value;
            RaisePropertyChanged();
        }
    }

    public double PolicyAnnouncementBoost
    {
        get => Parameters?.PolicyAnnouncementBoost ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.PolicyAnnouncementBoost = value;
            RaisePropertyChanged();
        }
    }

    public double BaseTurnout
    {
        get => Parameters?.BaseTurnout ?? 0;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.BaseTurnout = value;
            RaisePropertyChanged();
        }
    }

    public bool WeeklyTick
    {
        get => Parameters?.WeeklyTick ?? false;
        set
        {
            if (Parameters is null)
            {
                return;
            }

            Parameters.WeeklyTick = value;
            RaisePropertyChanged();
        }
    }

    public void AttachParameters(SimulationParameters parameters)
    {
        Parameters = parameters;
    }
}
