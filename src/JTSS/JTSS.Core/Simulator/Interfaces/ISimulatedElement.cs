namespace JTSS.Core.Simulator.Interfaces;

/// <summary>
/// Defines an element that participates in the simulation and needs to be updated over time.
/// </summary>
public interface ISimulatedElement
{
    /// <summary>
    /// Called by the simulator on each time step to allow the element to update its internal state.
    /// </summary>
    /// <param name="deltaTime">The amount of simulation time that has passed since the last update.</param>
    void Update(TimeSpan deltaTime);
}