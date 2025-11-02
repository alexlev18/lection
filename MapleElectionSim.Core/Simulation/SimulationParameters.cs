namespace MapleElectionSim.Core.Simulation;

/// <summary>
/// Tunable configuration influencing the simulation engine.
/// </summary>
public class SimulationParameters
{
    public bool WeeklyTick { get; set; }

    public double EconomicGrowth { get; set; } = 0.02;

    public double CostOfLivingIndex { get; set; } = 1.0;

    public double MediaImpactWeight { get; set; } = 0.5;

    public double ScandalWeight { get; set; } = 0.75;

    public double PolicyAnnouncementBoost { get; set; } = 0.4;

    public double BaseTurnout { get; set; } = 0.6;

    public double EnthusiasmDecay { get; set; } = 0.02;

    public double FundraisingMultiplier { get; set; } = 1.0;
}
