using System.Collections.Generic;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Represents a zone, which is a named collection of track paths defining a specific area of the network.
/// </summary>
public interface IZone : ITrackNetworkElement
{
    /// <summary>
    /// The collection of track paths that constitute this zone.
    /// </summary>
    IReadOnlyList<ITrackPath> TrackPaths { get; }

    /// <summary>
    /// Adds a track path to the zone.
    /// </summary>
    /// <param name="path">The track path to add.</param>
    void AddPath(ITrackPath path);

    /// <summary>
    /// Determines if a given track path intersects with any of the paths within this zone.
    /// </summary>
    /// <param name="path">The path to check for intersection.</param>
    /// <returns>True if the path intersects the zone, false otherwise.</returns>
    bool IntersectsWith(ITrackPath path);
}