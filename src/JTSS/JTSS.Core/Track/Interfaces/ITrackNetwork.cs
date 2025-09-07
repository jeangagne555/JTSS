using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    ITrackSegment AddTrackSegment(string id, double length);
    IStraightNode AddStraightNode(string id);
    ISwitchNode AddSwitchNode(string id);
    ICrossingNode AddCrossingNode(string id);
    IBorderNode AddBorderNode(string id);
    IZone AddZone(string id);
}
