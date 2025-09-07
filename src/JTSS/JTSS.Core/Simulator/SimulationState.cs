using JTSS.Core.Simulator.Enums;
using JTSS.Core.Simulator.Interfaces;

namespace JTSS.Core.Simulator;

/// <summary>
/// A concrete implementation of the simulation state.
/// This is internal to ensure it is only managed by the Simulator class.
/// </summary>
internal class SimulationState : ISimulationState
{
    public TimeSpan CurrentTime { get; internal set; } = TimeSpan.Zero;
    public SimulationStatus Status { get; internal set; } = SimulationStatus.Stopped;
}
