namespace JTSS.Core.Simulator.Interfaces;

/// <summary>
/// Defines the main simulation engine that manages the clock and updates all simulated elements.
/// </summary>
public interface ISimulationEngine
{
    /// <summary>
    /// The current state and clock of the simulation.
    /// </summary>
    ISimulationState State { get; }

    /// <summary>
    /// Registers a new element to be included in the simulation's update loop.
    /// </summary>
    /// <param name="element">The element to register.</param>
    void RegisterElement(ISimulatedElement element);

    /// <summary>
    /// Advances the simulation by a single, fixed time step.
    /// </summary>
    void Step();
}