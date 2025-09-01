using JTSS.Core.Track.Enums;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Represents a continuous path between two points on the track network.
/// A path is immutable; moving it creates a new path instance.
/// </summary>
public interface ITrackPath
{
    /// <summary>
    /// The starting position of the path.
    /// </summary>
    ITrackPosition StartPosition { get; }

    /// <summary>
    /// The ending position of the path.
    /// </summary>
    ITrackPosition EndPosition { get; }

    /// <summary>
    /// The total length of the path in meters.
    /// </summary>
    double Length { get; }

    /// <summary>
    /// Moves the entire path by a specified distance in a given direction.
    /// </summary>
    /// <param name="direction">The direction to move (Forward or Backward).</param>
    /// <param name="distanceInMeters">The distance to move. Must be non-negative.</param>
    /// <returns>
    /// A new ITrackPath instance representing the moved path, or null if either
    /// end of the path would move off the end of the track.
    /// </returns>
    ITrackPath? Move(PathDirection direction, double distanceInMeters);

    /// <summary>
    /// Merges this path with another path that starts exactly where this one ends.
    /// </summary>
    /// <param name="adjoiningPath">The path to append to the end of this one.</param>
    /// <returns>A new ITrackPath that represents the combination of the two paths.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided path does not start
    /// at the exact end position of this path.</exception>
    ITrackPath Merge(ITrackPath adjoiningPath);

    /// <summary>
    /// Splits this path into two new paths at a specified distance from an origin point.
    /// </summary>
    /// <param name="distanceFromOrigin">The distance at which to split the path. Must be between 0 and the path's Length.</param>
    /// <param name="origin">The point from which to measure the distance (FromStart or FromEnd).</param>
    /// <returns>A tuple containing the first and second path segments after the split.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the distance is negative or greater than the path's length.</exception>
    (ITrackPath first, ITrackPath second) Split(double distanceFromOrigin, SplitOrigin origin);

    /// <summary>
    /// Determines if this path shares any common track portion with another path.
    /// </summary>
    /// <param name="otherPath">The path to check for intersection.</param>
    /// <returns>True if the paths touch or overlap, false otherwise.</returns>
    bool IntersectsWith(ITrackPath otherPath);

    /// <summary>
    /// Performs a fast check to see if this path occupies any part of a given track segment.
    /// </summary>
    /// <param name="segment">The track segment to check for.</param>
    /// <returns>True if the path starts, ends, or passes through the segment.</returns>
    bool IsOnSegment(ITrackSegment segment);
}
