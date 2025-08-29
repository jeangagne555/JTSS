using JTSS.Core.Enums;
using System.Collections.Generic;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a node in the track network to which track segments can be connected.
    /// </summary>
    public interface ITrackNode
    {
        /// <summary>
        /// Gets the list of segments connected to this node, including which end of each segment is attached.
        /// </summary>
        IReadOnlyList<(ITrackSegment Segment, SegmentEnd End)> ConnectedSegments { get; }
    }
}