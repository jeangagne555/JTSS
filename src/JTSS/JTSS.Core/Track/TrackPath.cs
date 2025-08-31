using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Track;


/// <summary>
/// A concrete implementation of a track path.
/// </summary>
public class TrackPath : ITrackPath
{
    public ITrackPosition StartPosition { get; }
    public ITrackPosition EndPosition { get; }
    public double Length { get; }

    // Private fields to store the pre-calculated orientation
    private readonly ITrackNavigator _navigator;
    private readonly TravelDirection _startTravelDirectionForward;
    private readonly TravelDirection _endTravelDirectionForward;

    /// <summary>
    /// Creates a new TrackPath. The constructor calculates and stores the path's orientation.
    /// </summary>
    /// <param name="startPosition">The starting position of the path.</param>
    /// <param name="endPosition">The ending position of the path.</param>
    /// <param name="navigator">The track navigator service, required for calculations.</param>
    /// <exception cref="ArgumentException">Thrown if no valid path exists between the start and end positions.</exception>
    public TrackPath(ITrackPosition startPosition, ITrackPosition endPosition, ITrackNavigator navigator)
    {
        StartPosition = startPosition ?? throw new ArgumentNullException(nameof(startPosition));
        EndPosition = endPosition ?? throw new ArgumentNullException(nameof(endPosition));
        _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));

        // 1. Calculate the length of the path.
        double? length = _navigator.GetDistanceBetween(StartPosition, EndPosition);
        if (!length.HasValue)
        {
            throw new ArgumentException("No valid path exists between the specified start and end positions.");
        }
        Length = length.Value;

        // 2. Determine the "Forward" TravelDirection for the StartPosition.
        _startTravelDirectionForward = DetermineForwardDirection(StartPosition, EndPosition);

        // 3. Determine the "Forward" TravelDirection for the EndPosition.
        _endTravelDirectionForward = DetermineForwardDirection(EndPosition, StartPosition, true);
    }

    /// <inheritdoc/>
    public ITrackPath? Move(PathDirection direction, double distanceInMeters)
    {
        if (distanceInMeters < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceInMeters), "Distance must be non-negative.");
        }
        if (distanceInMeters == 0)
        {
            return this;
        }

        // Determine the absolute TravelDirection for both ends based on the path's relative direction.
        TravelDirection startMoveDirection;
        TravelDirection endMoveDirection;

        if (direction == PathDirection.Forward)
        {
            startMoveDirection = _startTravelDirectionForward;
            endMoveDirection = _endTravelDirectionForward;
        }
        else // Backward
        {
            startMoveDirection = OppositeDirection(_startTravelDirectionForward);
            endMoveDirection = OppositeDirection(_endTravelDirectionForward);
        }

        // Move both the start and end positions.
        var newStartPosition = _navigator.MovePosition(StartPosition, startMoveDirection, distanceInMeters);
        var newEndPosition = _navigator.MovePosition(EndPosition, endMoveDirection, distanceInMeters);

        // If either move fails (goes off the track), the entire operation fails.
        if (newStartPosition == null || newEndPosition == null)
        {
            return null;
        }

        // Return a new, immutable path instance with the new positions.
        return new TrackPath(newStartPosition, newEndPosition, _navigator);
    }

    /// <inheritdoc/>
    public ITrackPath Merge(ITrackPath adjoiningPath)
    {
        ArgumentNullException.ThrowIfNull(adjoiningPath);

        // The core validation: the two paths must be perfectly continuous.
        // We can rely on the value-based equality of the TrackPosition record.
        if (this.EndPosition != adjoiningPath.StartPosition)
        {
            throw new ArgumentException("Paths are not adjoining. The end of the first path must be the same as the start of the second path.", nameof(adjoiningPath));
        }

        // The constructor does the heavy lifting of calculating the new total length
        // and re-validating the newly formed, longer path.
        return new TrackPath(this.StartPosition, adjoiningPath.EndPosition, this._navigator);
    }

    /// <summary>
    /// Determines the TravelDirection that moves point A towards point B.
    /// </summary>
    private TravelDirection DetermineForwardDirection(ITrackPosition posA, ITrackPosition posB, bool isEndPoint = false)
    {
        // On the same segment, the logic is simple.
        if (posA.Segment.Id == posB.Segment.Id)
        {
            bool movesRight = posB.DistanceFromLeftEnd > posA.DistanceFromLeftEnd;
            return isEndPoint ? (movesRight ? TravelDirection.LeftToRight : TravelDirection.RightToLeft)
                              : (movesRight ? TravelDirection.LeftToRight : TravelDirection.RightToLeft);
        }

        // On different segments, we test a tiny move in each direction from posA
        // and see which one gets closer to posB.
        const double Epsilon = 0.01;

        ITrackPosition? posAfterMovingRight = _navigator.MovePosition(posA, TravelDirection.LeftToRight, Epsilon);
        ITrackPosition? posAfterMovingLeft = _navigator.MovePosition(posA, TravelDirection.RightToLeft, Epsilon);

        double? distAfterRight = posAfterMovingRight != null ? _navigator.GetDistanceBetween(posAfterMovingRight, posB) : null;
        double? distAfterLeft = posAfterMovingLeft != null ? _navigator.GetDistanceBetween(posAfterMovingLeft, posB) : null;

        double originalDist = _navigator.GetDistanceBetween(posA, posB)!.Value;

        // Logic for the start point: Forward is the direction that REDUCES the distance to the end.
        if (!isEndPoint)
        {
            if (distAfterRight.HasValue && distAfterRight < originalDist) return TravelDirection.LeftToRight;
            if (distAfterLeft.HasValue && distAfterLeft < originalDist) return TravelDirection.RightToLeft;
        }
        // Logic for the end point: Forward is the direction that INCREASES the distance to the start.
        else
        {
            if (distAfterRight.HasValue && distAfterRight > originalDist) return TravelDirection.LeftToRight;
            if (distAfterLeft.HasValue && distAfterLeft > originalDist) return TravelDirection.RightToLeft;
        }

        // Fallback for edge cases (e.g., at the very end of a segment)
        if (posAfterMovingLeft == null) return TravelDirection.LeftToRight;
        return TravelDirection.RightToLeft;
    }

    private static TravelDirection OppositeDirection(TravelDirection dir)
    {
        return dir == TravelDirection.LeftToRight ? TravelDirection.RightToLeft : TravelDirection.LeftToRight;
    }
}
