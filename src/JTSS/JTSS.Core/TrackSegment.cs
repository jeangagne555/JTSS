using JTSS.Core.Enums;
using JTSS.Core.Interfaces;

namespace JTSS.Core
{
    /// <summary>
    /// Represents a segment of track connecting two nodes.
    /// </summary>
    public class TrackSegment : ITrackSegment
    {
        /// <inheritdoc/>
        public double Length { get; }

        /// <inheritdoc/>
        public ITrackNode StartNode { get; }

        /// <inheritdoc/>
        public ITrackNode EndNode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackSegment"/> class.
        /// </summary>
        /// <param name="length">The length of the segment in meters.</param>
        /// <param name="startNode">The node at the start of the segment.</param>
        /// <param name="endNode">The node at the end of the segment.</param>
        public TrackSegment(double length, ITrackNode startNode, ITrackNode endNode)
        {
            Length = length;
            StartNode = startNode;
            EndNode = endNode;
        }

        /// <summary>
        /// Gets the next segment connected to the specified end of this segment.
        /// </summary>
        /// <param name="fromEnd">The end of this segment from which to find the next segment.</param>
        /// <returns>
        /// A tuple comprising the next <see cref="ITrackSegment"/> and the <see cref="SegmentEnd"/> 
        /// indicating which end of the segment is next, or null if there is no next segment.
        /// </returns>
        public (ITrackSegment Segment, SegmentEnd End)? GetNextSegment(SegmentEnd fromEnd)
        {
            // Determine the node we are moving toward
            ITrackNode nextNode = fromEnd == SegmentEnd.Start ? EndNode : StartNode;

            // Find the connection at this node that is NOT this segment
            foreach (var (segment, end) in nextNode.ConnectedSegments)
            {
                if (!ReferenceEquals(segment, this))
                {
                    // If the node is a switch, it may have logic to select the correct segment
                    if (nextNode is ISwitchNode switchNode)
                    {
                        var next = switchNode.GetNextSegment((this, end));
                        if (next != null)
                            return next;
                    }
                    else
                    {
                        // For straight/crossing nodes, just return the other segment
                        return (segment, end);
                    }
                }
            }
            return null;
        }
    }
}