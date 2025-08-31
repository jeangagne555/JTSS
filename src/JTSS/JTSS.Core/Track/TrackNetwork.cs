using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track;

/// <summary>
/// Manages the collection of all track segments and nodes.
/// </summary>
public class TrackNetwork : ITrackNetwork
{
    private readonly Dictionary<string, ITrackNetworkElement> _elements = new();

    public ITrackNetworkElement? GetElementById(string id)
    {
        _elements.TryGetValue(id, out var element);
        return element;
    }

    public ITrackSegment? GetSegmentById(string id)
    {
        return GetElementById(id) as ITrackSegment;
    }

    public ITrackNode? GetNodeById(string id)
    {
        return GetElementById(id) as ITrackNode;
    }

    public ITrackSegment AddTrackSegment(string id, double length)
    {
        var segment = new TrackSegment(id, length);
        RegisterElement(segment);
        return segment;
    }

    public IStraightNode AddStraightNode(string id)
    {
        var node = new StraightNode(id);
        RegisterElement(node);
        return node;
    }

    public ISwitchNode AddSwitchNode(string id)
    {
        var node = new SwitchNode(id);
        RegisterElement(node);
        return node;
    }

    public ICrossingNode AddCrossingNode(string id)
    {
        var node = new CrossingNode(id);
        RegisterElement(node);
        return node;
    }

    public IBorderNode AddBorderNode(string id)
    {
        var node = new BorderNode(id);
        RegisterElement(node);
        return node;
    }

    private void RegisterElement(ITrackNetworkElement element)
    {
        if (!_elements.TryAdd(element.Id, element))
        {
            throw new ArgumentException($"An element with ID '{element.Id}' already exists in the network.");
        }
    }
}
