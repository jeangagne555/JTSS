using JTSS.Core.Interfaces;
using JTSS.Core.Simulator.Interfaces;
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
    /// Places the train onto the track network for the first time.
    /// </summary>
    /// <param name="headPosition">The position of the very front of the train.</param>
    void Place(ITrackPosition headPosition);
}