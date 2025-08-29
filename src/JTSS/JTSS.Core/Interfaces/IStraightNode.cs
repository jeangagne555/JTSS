using JTSS.Core.Enums;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a straight node that connects two track segments.
    /// </summary>
    public interface IStraightNode : ITrackNode
    {
        /// <summary>
        /// Gets the first connection (segment and end) of the straight node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) ConnectionA { get; }

        /// <summary>
        /// Gets the second connection (segment and end) of the straight node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) ConnectionB { get; }
    }
}