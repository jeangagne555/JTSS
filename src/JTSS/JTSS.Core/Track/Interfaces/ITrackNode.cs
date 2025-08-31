using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a node that connects track segments.
/// </summary>
public interface ITrackNode : ITrackNetworkElement
{
    IReadOnlyList<ITrackSegment> Connections { get; }
    IEnumerable<ITrackSegment> GetValidPaths(ITrackSegment fromSegment);
}
