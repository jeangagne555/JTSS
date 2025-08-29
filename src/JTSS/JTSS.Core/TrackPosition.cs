using JTSS.Core.Interfaces;
using JTSS.Core.Enums;

namespace JTSS.Core
{
    /// <summary>
    /// Represents a dimensionless position on a specific track segment.
    /// </summary>
    public class TrackPosition : ITrackPosition
    {
        /// <inheritdoc/>
        public ITrackSegment Segment { get; }

        /// <inheritdoc/>
        public double Offset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackPosition"/> class.
        /// </summary>
        /// <param name="segment">The track segment.</param>
        /// <param name="offset">The distance from the start of the segment, in meters.</param>
        public TrackPosition(ITrackSegment segment, double offset)
        {
            Segment = segment;
            Offset = offset;
        }

        /// <inheritdoc/>
        public ITrackPosition? Move(double distance, SegmentEnd direction)
        {
            if (distance < 0) throw new ArgumentOutOfRangeException(nameof(distance));

            ITrackSegment currentSegment = Segment;
            double currentOffset = Offset;

            while (distance > 0)
            {
                double remainingInSegment = direction == SegmentEnd.End
                    ? currentSegment.Length - currentOffset
                    : currentOffset;

                if (distance <= remainingInSegment)
                {
                    double newOffset = direction == SegmentEnd.End
                        ? currentOffset + distance
                        : currentOffset - distance;
                    return new TrackPosition(currentSegment, newOffset);
                }
                else
                {
                    distance -= remainingInSegment;
                    var next = currentSegment.GetNextSegment(direction);
                    if (next == null)
                        return null;

                    currentSegment = next.Value.Segment;
                    currentOffset = next.Value.End == SegmentEnd.Start ? 0 : currentSegment.Length;
                    direction = next.Value.End;
                }
            }

            return new TrackPosition(currentSegment, currentOffset);
        }

        /// <inheritdoc/>
        public double? DistanceTo(ITrackPosition other)
        {
            // Same segment: just subtract offsets
            if (Segment == other.Segment)
                return Math.Abs(Offset - other.Offset);

            // Try forward traversal from this position
            double distance = 0;
            var current = this;
            var direction = Offset < other.Offset ? SegmentEnd.End : SegmentEnd.Start;
            var visited = new HashSet<ITrackSegment> { Segment };

            while (true)
            {
                double remaining = direction == SegmentEnd.End
                    ? current.Segment.Length - current.Offset
                    : current.Offset;

                var next = current.Segment.GetNextSegment(direction);
                if (next == null || !visited.Add(next.Value.Segment))
                    break;

                distance += remaining;
                if (next.Value.Segment == other.Segment)
                {
                    distance += direction == SegmentEnd.End ? other.Offset : next.Value.Segment.Length - other.Offset;
                    return distance;
                }

                current = new TrackPosition(next.Value.Segment,
                    next.Value.End == SegmentEnd.Start ? 0 : next.Value.Segment.Length);
                direction = next.Value.End;
            }

            // Try reverse traversal from other to this
            distance = 0;
            current = (TrackPosition)other;
            direction = other.Offset < Offset ? SegmentEnd.End : SegmentEnd.Start;
            visited = new HashSet<ITrackSegment> { other.Segment };

            while (true)
            {
                double remaining = direction == SegmentEnd.End
                    ? current.Segment.Length - current.Offset
                    : current.Offset;

                var next = current.Segment.GetNextSegment(direction);
                if (next == null || !visited.Add(next.Value.Segment))
                    break;

                distance += remaining;
                if (next.Value.Segment == Segment)
                {
                    distance += direction == SegmentEnd.End ? Offset : next.Value.Segment.Length - Offset;
                    return distance;
                }

                current = new TrackPosition(next.Value.Segment,
                    next.Value.End == SegmentEnd.Start ? 0 : next.Value.Segment.Length);
                direction = next.Value.End;
            }

            // Not reachable
            return null;
        }
    }
}