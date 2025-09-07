namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Manages the collection of all track segments and nodes, ensuring ID uniqueness.
/// </summary>
public interface ITrackNetwork
{
    ITrackNetworkElement? GetElementById(string id);
    ITrackSegment? GetSegmentById(string id);
    ITrackNode? GetNodeById(string id);
    IZone? GetZoneById(string id);
    ITrackSegment AddTrackSegment(string id, double length, string? name = null);
    IStraightNode AddStraightNode(string id, string? name = null);
    ISwitchNode AddSwitchNode(string id, string? name = null);
    ICrossingNode AddCrossingNode(string id, string? name = null);
    IBorderNode AddBorderNode(string id, string? name = null);
    IZone AddZone(string id, string? name = null);
}