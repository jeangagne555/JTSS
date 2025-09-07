using JTSS.Core.Simulator.Interfaces;

namespace JTSS.Core.Simulator;

/// <summary>
/// A simple, discrete time-step simulator.
/// </summary>
public class SimulationEngine : ISimulationEngine
{
    private readonly SimulationState _simulationState = new();
    private readonly List<ISimulatedElement> _elements = new();
    private readonly TimeSpan _timeStep;

    public ISimulationState State => _simulationState;

    /// <summary>
    /// Initializes a new simulator.
    /// </summary>
    /// <param name="timeStep">The fixed amount of time to advance the clock on each step.</param>
    public SimulationEngine(TimeSpan timeStep)
    {
        if (timeStep <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeStep), "Time step must be positive.");
        }
        _timeStep = timeStep;
    }

    /// <inheritdoc/>
    public void RegisterElement(ISimulatedElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (!_elements.Contains(element))
        {
            _elements.Add(element);
        }
    }

    /// <inheritdoc/>
    public void Step()
    {
        // 1. Advance the clock.
        _simulationState.CurrentTime += _timeStep;

        // 2. Update all registered elements, passing them the delta time.
        foreach (var element in _elements)
        {
            element.Update(_timeStep);
        }
    }
}