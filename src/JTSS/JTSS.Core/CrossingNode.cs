using JTSS.Core.Enums;
using JTSS.Core.Interfaces;
using System.Collections.Generic;

namespace JTSS.Core
{
    /// <summary>
    /// Represents a crossing node that allows two track lines to cross, connecting four segments.
    /// </summary>
    public class CrossingNode : ICrossingNode
    {
        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Connection1 { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Connection2 { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Connection3 { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Connection4 { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossingNode"/> class.
        /// </summary>
        /// <param name="connection1">The first connection (segment and end).</param>
        /// <param name="connection2">The second connection (segment and end).</param>
        /// <param name="connection3">The third connection (segment and end).</param>
        /// <param name="connection4">The fourth connection (segment and end).</param>
        public CrossingNode(
            (ITrackSegment, SegmentEnd) connection1,
            (ITrackSegment, SegmentEnd) connection2,
            (ITrackSegment, SegmentEnd) connection3,
            (ITrackSegment, SegmentEnd) connection4)
        {
            Connection1 = connection1;
            Connection2 = connection2;
            Connection3 = connection3;
            Connection4 = connection4;
        }

        /// <inheritdoc/>
        public IReadOnlyList<(ITrackSegment Segment, SegmentEnd End)> ConnectedSegments =>
            new List<(ITrackSegment, SegmentEnd)> { Connection1, Connection2, Connection3, Connection4 };
    }
}