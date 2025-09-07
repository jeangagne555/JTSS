using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track;

public class StraightNode : IStraightNode
{
    public string Id { get; }
    public string? Name { get; set; }
    private readonly List<ITrackSegment> _connections = new(2);
    public IReadOnlyList<ITrackSegment> Connections => _connections;

    public StraightNode(string id, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = name;
    }

    // ... rest of the file is unchanged
    public void Connect(TrackConnection connectionA, TrackConnection connectionB)
    {
        if (_connections.Any())
            throw new InvalidOperationException($"Straight node '{Id}' has already been connected.");

        _connections.Add(connectionA.Segment);
        _connections.Add(connectionB.Segment);

        connectionA.Segment.SetNodeAt(connectionA.End, this);
        connectionB.Segment.SetNodeAt(connectionB.End, this);
    }

    public IEnumerable<ITrackSegment> GetValidPaths(ITrackSegment fromSegment)
    {
        if (_connections.Count == 2)
        {
            if (fromSegment == _connections[0]) yield return _connections[1];
            else if (fromSegment == _connections[1]) yield return _connections[0];
        }
    }
}