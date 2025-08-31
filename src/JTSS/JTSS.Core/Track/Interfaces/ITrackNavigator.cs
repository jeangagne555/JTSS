using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Provides services for traversing the track network.
/// </summary>
public interface ITrackNavigator
{
    /// <summary>
    /// Finds the next track segment and the new direction of travel.
    /// </summary>
    /// <param name="currentSegment">The segment the train is currently on.</param>
    /// <param name="currentDirection">The direction the train is traveling along the current segment.</param>
    /// <returns>
    /// A NavigationResult containing the next segment and the new travel direction,
    /// or null if the track ends or the path is blocked.
    /// </returns>
    NavigationResult? NavigateToNextSegment(ITrackSegment currentSegment, TravelDirection currentDirection);

    /// <summary>
    /// Moves a position along the track by a specified distance and direction.
    /// This operation can cross over nodes to adjacent track segments.
    /// </summary>
    /// <param name="startPosition">The initial position on the track.</param>
    /// <param name="direction">The direction of travel from the starting position.</param>
    /// <param name="distanceInMeters">The total distance to move. Must be a positive number.</param>
    /// <returns>
    /// A new TrackPosition representing the final location, or null if the move
    /// travels off the end of the track before covering the full distance.
    /// </returns>
    ITrackPosition? MovePosition(ITrackPosition startPosition, TravelDirection direction, double distanceInMeters);

    /// <summary>
    /// Calculates the shortest distance between two positions on the track network.
    /// </summary>
    /// <param name="positionA">The first position.</param>
    /// <param name="positionB">The second position.</param>
    /// <returns>
    /// The shortest distance in meters between the two points, or null if no valid path exists
    /// (e.g., disconnected tracks or a switch set against the path).
    /// </returns>
    double? GetDistanceBetween(ITrackPosition positionA, ITrackPosition positionB);
}
