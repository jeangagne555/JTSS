namespace JTSS.Core.Track.Enums;

/// <summary>
/// Specifies the direction of movement relative to a TrackPath's orientation.
/// </summary>
public enum PathDirection
{
    /// <summary>
    /// Moves the path from its StartPosition towards its EndPosition.
    /// </summary>
    Forward,

    /// <summary>
    /// Moves the path from its EndPosition towards its StartPosition.
    /// </summary>
    Backward
}