using JTSS.Core.Interfaces;
using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Trains.Interfaces;

/// <summary>
/// Defines a train within the simulation, which is an operational element
/// that has a physical length and occupies a specific path on the track network.
/// </summary>
public interface ITrain : IIdentifiableElement, ISimulatedElement
{
    /// <summary>
    /// The total length of the train in meters.
    /// </summary>
    double Length { get; }

    /// <summary>
    /// The current path on the track network that the train occupies.
    /// This will be null if the train has not yet been placed in the world.
    /// </summary>
    ITrackPath? Path { get; }

    /// <summary>
    /// The position of the head of the train. Returns null if the train is not on the track.
    /// </summary>
    ITrackPosition? Head { get; }

    /// <summary>
    /// The position of the tail of the train. Returns null if the train is not on the track.
    /// </summary>
    ITrackPosition? Tail { get; }

    /// <summary>
    /// Places the train onto the track network for the first time. This action creates the
    /// initial path that the train occupies, based on its length.
    /// </summary>
    /// <param name="headPosition">The position of the very front of the train.</param>
    /// <param name="initialDirection">The initial direction the train is facing.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the train has already been placed, or if the train is too long
    /// to fit on the track from the specified head position.
    /// </exception>
    void Place(ITrackPosition headPosition, TravelDirection initialDirection);
}