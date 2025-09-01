namespace JTSS.Core.Track.Enums;

/// <summary>
/// Specifies the origin point from which a split distance is measured on a TrackPath.
/// </summary>
public enum SplitOrigin
{
    /// <summary>
    /// The distance is measured forward from the StartPosition of the path.
    /// </summary>
    FromStart,

    /// <summary>
    /// The distance is measured backward from the EndPosition of the path.
    /// </summary>
    FromEnd
}
