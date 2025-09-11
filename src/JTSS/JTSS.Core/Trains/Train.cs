using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Trains.Interfaces;

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

    /// <inheritdoc/>
    public ITrackPosition? Head => Path?.EndPosition;

    /// <inheritdoc/>
    public ITrackPosition? Tail => Path?.StartPosition;

    private readonly ISimulationState _simulationState;
    private readonly ITrackNavigator _navigator;

    /// <summary>
    /// Initializes a new instance of the Train class.
    /// </summary>
    /// <param name="id">The unique identifier for the train.</param>
    /// <param name="length">The length of the train in meters.</param>
    /// <param name="simulationState">The state of the simulation.</param>
    /// <param name="name">An optional, user-friendly name for the train.</param>
    /// <summary>
    /// Initializes a new instance of the Train class.
    /// </summary>
    /// <param name="id">The unique identifier for the train.</param>
    /// <param name="length">The length of the train in meters.</param>
    /// <param name="simulationState">The state of the simulation.</param>
    /// <param name="navigator">The track navigator service, required for placement and movement.</param>
    /// <param name="name">An optional, user-friendly name for the train.</param>
    public Train(string id, double length, ISimulationState simulationState, ITrackNavigator navigator, string? name = null)
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
        _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator)); // Store the navigator
    }

    /// <inheritdoc/>
    public void Update(TimeSpan deltaTime)
    {
        // Train movement logic will be implemented here in future steps.
        // We can now access _simulationState.CurrentTime if needed.
    }

    /// <inheritdoc/>
    public void Place(ITrackPosition headPosition, TravelDirection initialDirection)
    {
        ArgumentNullException.ThrowIfNull(headPosition);

        // 1. Ensure the train has not already been placed.
        if (this.Path != null)
        {
            throw new InvalidOperationException($"Train '{Id}' has already been placed on the track.");
        }

        // 2. Determine the direction to move backward to find the tail.
        var tailDirection = OppositeDirection(initialDirection);

        // 3. Use the navigator to find the tail position by moving backward from the head.
        ITrackPosition? tailPosition = _navigator.MovePosition(headPosition, tailDirection, this.Length);

        // 4. Validate that the placement was successful.
        if (tailPosition == null)
        {
            throw new InvalidOperationException($"Cannot place train '{Id}'. The train is too long ({Length}m) to fit on the track from the specified head position.");
        }

        // 5. Create the initial path. Remember, the Tail is the StartPosition and the Head is the EndPosition.
        this.Path = new TrackPath(tailPosition, headPosition, _navigator);
    }

    /// <summary>
    /// Moves the train along its path by a specified distance. This method is protected
    /// and is intended to be called by the Update loop or for testing via a subclass.
    /// </summary>
    /// <param name="direction">The direction to move (Forward or Backward) relative to the train's path.</param>
    /// <param name="distanceInMeters">The distance to move. Must be non-negative.</param>
    /// <returns>True if the move was successful; false if the train moved off the end of the track.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the train has not been placed on the track yet.</exception>
    protected virtual bool Move(PathDirection direction, double distanceInMeters)
    {
        if (this.Path == null)
        {
            throw new InvalidOperationException("Cannot move a train that has not been placed on the track.");
        }
        if (distanceInMeters < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceInMeters), "Distance must be non-negative.");
        }
        if (distanceInMeters == 0)
        {
            return true;
        }

        var newPath = this.Path.Move(direction, distanceInMeters);
        this.Path = newPath;

        return this.Path != null;
    }

    /// <summary>
    /// Gets the opposite travel direction.
    /// </summary>
    private static TravelDirection OppositeDirection(TravelDirection direction)
    {
        return direction == TravelDirection.LeftToRight
            ? TravelDirection.RightToLeft
            : TravelDirection.LeftToRight;
    }
}