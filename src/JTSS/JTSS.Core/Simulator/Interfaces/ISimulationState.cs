using JTSS.Core.Simulator.Enums;

namespace JTSS.Core.Simulator.Interfaces;

/// <summary>
/// Defines the state of the simulation, primarily the master clock.
/// </summary>
public interface ISimulationState
{
    /// <summary>
    /// The current time elapsed since the simulation started.
    /// </summary>
    TimeSpan CurrentTime { get; }

    /// <summary>
    /// The current status of the simulation (e.g., Running, Paused).
    /// </summary>
    SimulationStatus Status { get; }
}