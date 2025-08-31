using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a precise position on a specific track segment.
/// </summary>
public interface ITrackPosition
{
    /// <summary>
    /// The track segment on which the position exists.
    /// </summary>
    ITrackSegment Segment { get; }

    /// <summary>
    /// The distance in meters from the left end of the segment.
    /// This is the canonical measurement for a position on the segment.
    /// </summary>
    double DistanceFromLeftEnd { get; }
}