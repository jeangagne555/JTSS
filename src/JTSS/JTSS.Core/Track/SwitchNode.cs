using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track;

public class SwitchNode : ISwitchNode
{
    public string Id { get; }
    public ITrackSegment Facing { get; private set; }
    public ITrackSegment Trailing { get; private set; }
    public ITrackSegment Diverging { get; private set; }
    public SwitchState State { get; set; }

    private readonly List<ITrackSegment> _connections = new(3);
    public IReadOnlyList<ITrackSegment> Connections => _connections;

    public SwitchNode(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        State = SwitchState.Normal;
    }

    public void Connect(TrackConnection facing, TrackConnection trailing, TrackConnection diverging)
    {
        if (_connections.Any())
            throw new InvalidOperationException($"Switch node '{Id}' has already been connected.");

        Facing = facing.Segment;
        Trailing = trailing.Segment;
        Diverging = diverging.Segment;

        _connections.AddRange(new[] { Facing, Trailing, Diverging });

        Facing.SetNodeAt(facing.End, this);
        Trailing.SetNodeAt(trailing.End, this);
        Diverging.SetNodeAt(diverging.End, this);
    }

    public IEnumerable<ITrackSegment> GetValidPaths(ITrackSegment fromSegment)
    {
        if (fromSegment == Facing)
        {
            yield return State == SwitchState.Normal ? Trailing : Diverging;
        }
        else if (fromSegment == Trailing && State == SwitchState.Normal)
        {
            yield return Facing;
        }
        else if (fromSegment == Diverging && State == SwitchState.Reversed)
        {
            yield return Facing;
        }
    }
}