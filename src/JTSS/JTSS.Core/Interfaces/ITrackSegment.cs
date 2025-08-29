using JTSS.Core.Enums;
using System.Collections.Generic;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a segment of track in the network, connecting two nodes.
    /// </summary>
    public interface ITrackSegment
    {
        /// <summary>
        /// Gets the length of the track segment, in meters.
        /// </summary>
        double Length { get; }

        /// <summary>
        /// Gets the node connected to the start of the segment.
        /// </summary>
        ITrackNode StartNode { get; }

        /// <summary>
        /// Gets the node connected to the end of the segment.
        /// </summary>
        ITrackNode EndNode { get; }

        /// <summary>
        /// Returns the next segment and end when traveling from a given end, considering the current state of the network.
        /// </summary>
        /// <param name="fromEnd">The end of this segment from which travel starts.</param>
        /// <returns>The next segment and end, or null if no valid path exists.</returns>
        (ITrackSegment Segment, SegmentEnd End)? GetNextSegment(SegmentEnd fromEnd);
    }
}