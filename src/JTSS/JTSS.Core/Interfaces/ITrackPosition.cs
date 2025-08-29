namespace JTSS.Core.Interfaces
{
    /// <summary>
    /// Represents a dimensionless position on a specific track segment.
    /// </summary>
    public interface ITrackPosition
    {
        /// <summary>
        /// The segment on which this position lies.
        /// </summary>
        ITrackSegment Segment { get; }

        /// <summary>
        /// The distance from the start of the segment, in meters.
        /// </summary>
        double Offset { get; }
    }
}