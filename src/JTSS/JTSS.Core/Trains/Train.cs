using JTSS.Core.Interfaces;
using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Trains.Interfaces;
using System;

namespace JTSS.Core.Trains;

/// <summary>
/// Represents a train within the simulation.
/// </summary>
public class Train : ITrain
{
    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public double Length { get; }

    /// <inheritdoc/>
    public ITrackPath? Path { get; private set; }

    private readonly ISimulationState _simulationState;

    /// <summary>
    /// Initializes a new instance of the Train class.
    /// </summary>
    /// <param name="id">The unique identifier for the train.</param>
    /// <param name="length">The length of the train in meters.</param>
    /// <param name="simulationState">The state of the simulation.</param>
    /// <param name="name">An optional, user-friendly name for the train.</param>
    public Train(string id, double length, ISimulationState simulationState, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Train length must be a positive number.");
        }

        Id = id;
        Name = name;
        Length = length;
        _simulationState = simulationState ?? throw new ArgumentNullException(nameof(simulationState));
    }

    /// <inheritdoc/>
    public void Update(TimeSpan deltaTime)
    {
        // Train movement logic will be implemented here in future steps.
        // We can now access _simulationState.CurrentTime if needed.
    }

    /// <inheritdoc/>
    public void Place(ITrackPosition headPosition)
    {
        // We will implement the logic to calculate the train's initial path later.
        throw new NotImplementedException();
    }
}