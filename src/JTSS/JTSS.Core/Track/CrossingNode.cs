using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track;

public class CrossingNode : ICrossingNode
{
    public string Id { get; }
    public string? Name { get; set; }
    private readonly List<ITrackSegment> _connections = new(4);
    public IReadOnlyList<ITrackSegment> Connections => _connections;

    private (ITrackSegment A, ITrackSegment B) _line1;
    private (ITrackSegment A, ITrackSegment B) _line2;

    public CrossingNode(string id, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = name;
    }

    // ... rest of the file is unchanged
    public void Connect(TrackConnection line1A, TrackConnection line1B, TrackConnection line2A, TrackConnection line2B)
    {
        if (_connections.Any())
            throw new InvalidOperationException($"Crossing node '{Id}' has already been connected.");

        _line1 = (line1A.Segment, line1B.Segment);
        _line2 = (line2A.Segment, line2B.Segment);

        _connections.AddRange(new[] { _line1.A, _line1.B, _line2.A, _line2.B });

        line1A.Segment.SetNodeAt(line1A.End, this);
        line1B.Segment.SetNodeAt(line1B.End, this);
        line2A.Segment.SetNodeAt(line2A.End, this);
        line2B.Segment.SetNodeAt(line2B.End, this);
    }

    public IEnumerable<ITrackSegment> GetValidPaths(ITrackSegment fromSegment)
    {
        if (fromSegment == _line1.A) yield return _line1.B;
        else if (fromSegment == _line1.B) yield return _line1.A;
        else if (fromSegment == _line2.A) yield return _line2.B;
        else if (fromSegment == _line2.B) yield return _line2.A;
    }
}