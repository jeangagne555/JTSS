using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Models;

/// <summary>
/// Represents a specific connection point on a track segment.
/// </summary>
/// <param name="Segment">The track segment being connected.</param>
/// <param name="End">The specific end (Left or Right) of the segment being connected.</param>
public record TrackConnection(ITrackSegment Segment, SegmentEnd End);
