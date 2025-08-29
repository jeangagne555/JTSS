using JTSS.Core.Enums;
using JTSS.Core.Interfaces;
using System.Collections.Generic;

namespace JTSS.Core
{
    /// <summary>
    /// Represents a switch node that connects three track segments.
    /// </summary>
    public class SwitchNode : ISwitchNode
    {
        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Facing { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Trailing { get; }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End) Diverging { get; }

        /// <inheritdoc/>
        public SwitchState State { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchNode"/> class.
        /// </summary>
        /// <param name="facing">The facing connection.</param>
        /// <param name="trailing">The trailing connection.</param>
        /// <param name="diverging">The diverging connection.</param>
        /// <param name="initialState">The initial state of the switch.</param>
        public SwitchNode(
            (ITrackSegment, SegmentEnd) facing,
            (ITrackSegment, SegmentEnd) trailing,
            (ITrackSegment, SegmentEnd) diverging,
            SwitchState initialState = SwitchState.Normal)
        {
            Facing = facing;
            Trailing = trailing;
            Diverging = diverging;
            State = initialState;
        }

        /// <inheritdoc/>
        public IReadOnlyList<(ITrackSegment Segment, SegmentEnd End)> ConnectedSegments =>
            new List<(ITrackSegment, SegmentEnd)> { Facing, Trailing, Diverging };

        /// <inheritdoc/>
        public bool CanTravel((ITrackSegment, SegmentEnd) from, (ITrackSegment, SegmentEnd) to)
        {
            if (State == SwitchState.Normal)
                return (from.Equals(Facing) && to.Equals(Trailing)) || (from.Equals(Trailing) && to.Equals(Facing));
            if (State == SwitchState.Reversed)
                return (from.Equals(Facing) && to.Equals(Diverging)) || (from.Equals(Diverging) && to.Equals(Facing));
            return false;
        }

        /// <inheritdoc/>
        public (ITrackSegment Segment, SegmentEnd End)? GetNextSegment((ITrackSegment Segment, SegmentEnd End) from)
        {
            // Determine the next segment based on the current state and the incoming segment
            if (from.Equals(Facing))
            {
                if (State == SwitchState.Normal)
                    return Trailing;
                if (State == SwitchState.Reversed)
                    return Diverging;
            }
            else if (from.Equals(Trailing) && State == SwitchState.Normal)
            {
                return Facing;
            }
            else if (from.Equals(Diverging) && State == SwitchState.Reversed)
            {
                return Facing;
            }
            return null;
        }
    }
}