using JTSS.Core.Track.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a segment of track with distinct Left and Right endpoints.
/// </summary>
public interface ITrackSegment : ITrackNetworkElement
{
    double Length { get; }
    ITrackNode LeftEndNode { get; }
    ITrackNode RightEndNode { get; }
    void SetNodeAt(SegmentEnd end, ITrackNode node);
    ITrackNode GetOppositeNode(ITrackNode node);
}
