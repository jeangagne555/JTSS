using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track;

/// <summary>
/// Represents a physical segment of track with a specific length and Left/Right endpoints.
/// </summary>
public class TrackSegment : ITrackSegment
{
    public string Id { get; }
    public double Length { get; }
    public ITrackNode LeftEndNode { get; private set; }
    public ITrackNode RightEndNode { get; private set; }

    public TrackSegment(string id, double length)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException("Length must be a positive number.", nameof(length));
        }
        Id = id;
        Length = length;
    }

    public void SetNodeAt(SegmentEnd end, ITrackNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (end == SegmentEnd.Left)
        {
            if (LeftEndNode != null) throw new InvalidOperationException($"Left end of segment '{Id}' is already connected to node '{LeftEndNode.Id}'.");
            LeftEndNode = node;
        }
        else // SegmentEnd.Right
        {
            if (RightEndNode != null) throw new InvalidOperationException($"Right end of segment '{Id}' is already connected to node '{RightEndNode.Id}'.");
            RightEndNode = node;
        }
    }

    public ITrackNode GetOppositeNode(ITrackNode node)
    {
        if (node == LeftEndNode) return RightEndNode;
        if (node == RightEndNode) return LeftEndNode;

        throw new ArgumentException($"The provided node '{node?.Id}' is not connected to this segment '{Id}'.", nameof(node));
    }
}
