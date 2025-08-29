using JTSS.Core.Enums;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a dimensionless position on a specific track segment.
    /// </summary>
    public interface ITrackPosition
    {
        /// <summary>
        /// The segment on which this position lies.
        /// </summary>
        ITrackSegment Segment { get; }

        /// <summary>
        /// The distance from the start of the segment, in meters.
        /// </summary>
        double Offset { get; }

        /// <summary>
        /// Moves the position along the track network by the specified distance and direction.
        /// </summary>
        /// <param name="distance">The distance to move. Must be non-negative.</param>
        /// <param name="direction">The direction to move: SegmentEnd.Start or SegmentEnd.End.</param>
        /// <returns>A new <see cref="ITrackPosition"/> at the new location, or null if the end of the network is reached.</returns>
        ITrackPosition? Move(double distance, SegmentEnd direction);

        /// <summary>
        /// Calculates the distance along the track network to another position.
        /// Returns null if the positions are not connected.
        /// </summary>
        /// <param name="other">The other position.</param>
        /// <returns>The distance in meters, or null if unreachable.</returns>
        double? DistanceTo(ITrackPosition other);
    }
}