using JTSS.Core.Enums;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a crossing node that allows two track lines to cross, connecting four segments.
    /// </summary>
    public interface ICrossingNode : ITrackNode
    {
        /// <summary>
        /// Gets the first connection (segment and end) of the crossing node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Connection1 { get; }

        /// <summary>
        /// Gets the second connection (segment and end) of the crossing node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Connection2 { get; }

        /// <summary>
        /// Gets the third connection (segment and end) of the crossing node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Connection3 { get; }

        /// <summary>
        /// Gets the fourth connection (segment and end) of the crossing node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Connection4 { get; }
    }
}