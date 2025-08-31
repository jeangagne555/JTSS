namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Represents a generic element within the track network that has a unique identifier.
/// </summary>
public interface ITrackNetworkElement
{
    /// <summary>
    /// The unique, user-defined identifier for this network element.
    /// </summary>
    string Id { get; }
}
