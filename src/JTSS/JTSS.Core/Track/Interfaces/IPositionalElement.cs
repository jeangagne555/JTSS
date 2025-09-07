namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a track network element that has a specific, fixed position on a track segment.
/// </summary>
public interface IPositionalElement : ITrackNetworkElement
{
    /// <summary>
    /// The precise position of this element on the track network.
    /// </summary>
    ITrackPosition Position { get; }
}
