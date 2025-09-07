using JTSS.Core.Interfaces;
using JTSS.Core.Simulator;
using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;

namespace JTSS.Core;

/// <summary>
/// Represents the top-level container for a complete simulation environment.
/// </summary>
public class World
{
    public ITrackNetwork Network { get; }

    /// <summary>
    /// The engine that manages the simulation clock and update loop.
    /// </summary>
    public ISimulationEngine SimulationEngine { get; }

    private readonly Dictionary<string, IIdentifiableElement> _operationalElements = new();

    public World(ITrackNetwork network, ISimulationEngine simulationEngine)
    {
        Network = network ?? throw new ArgumentNullException(nameof(network));
        SimulationEngine = simulationEngine ?? throw new ArgumentNullException(nameof(simulationEngine));
    }

    public IIdentifiableElement? GetOperationalElementById(string id)
    {
        _operationalElements.TryGetValue(id, out var element);
        return element;
    }

    internal void RegisterOperationalElement(IIdentifiableElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (!_operationalElements.TryAdd(element.Id, element))
        {
            throw new ArgumentException($"An operational element with ID '{element.Id}' already exists in the world.");
        }

        if (element is ISimulatedElement simulatedElement)
        {
            SimulationEngine.RegisterElement(simulatedElement);
        }
    }
}