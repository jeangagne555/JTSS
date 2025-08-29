using JTSS.Core.Enums;

namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a switch node that connects three track segments.
    /// </summary>
    public interface ISwitchNode : ITrackNode
    {
        /// <summary>
        /// Gets the facing connection (segment and end) of the switch node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Facing { get; }

        /// <summary>
        /// Gets the trailing connection (segment and end) of the switch node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Trailing { get; }

        /// <summary>
        /// Gets the diverging connection (segment and end) of the switch node.
        /// </summary>
        (ITrackSegment Segment, SegmentEnd End) Diverging { get; }

        /// <summary>
        /// Gets or sets the current state of the switch.
        /// </summary>
        SwitchState State { get; set; }

        /// <summary>
        /// Determines if travel is possible between two connections based on the current switch state.
        /// </summary>
        /// <param name="from">The starting connection.</param>
        /// <param name="to">The destination connection.</param>
        /// <returns>True if travel is allowed; otherwise, false.</returns>
        bool CanTravel((ITrackSegment, SegmentEnd) from, (ITrackSegment, SegmentEnd) to);

        /// <summary>
        /// Gets the next segment and end when entering this node from a specific segment and end, considering the switch state.
        /// </summary>
        /// <param name="from">The incoming segment and end.</param>
        /// <returns>The outgoing segment and end, or null if no valid path exists.</returns>
        (ITrackSegment Segment, SegmentEnd End)? GetNextSegment((ITrackSegment Segment, SegmentEnd End) from);
    }
}