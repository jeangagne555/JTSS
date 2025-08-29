using JTSS.Core.Enums;
using JTSS.Core.Interfaces;
using System.Collections.Generic;

namespace JTSS.Core
{
    /// <summary>
    /// Represents a straight node that connects two track segments.
    /// </summary>
    public class StraightNode : IStraightNode
    {
        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) ConnectionA { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) ConnectionB { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StraightNode"/> class.
        /// </summary>
        /// <param name="connectionA">The first connection (segment and end).</param>
        /// <param name="connectionB">The second connection (segment and end).</param>
        public StraightNode((ITrackSegment, SegmentEnd) connectionA, (ITrackSegment, SegmentEnd) connectionB)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;
        }

        /// <inheritdoc/>
        public IReadOnlyList<(ITrackSegment Segment, SegmentEnd End)> ConnectedSegments =>
            new List<(ITrackSegment, SegmentEnd)> { ConnectionA, ConnectionB };
    }
}