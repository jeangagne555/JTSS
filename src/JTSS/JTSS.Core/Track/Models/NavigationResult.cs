using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track.Models;

/// <summary>
/// Represents the result of a track navigation operation,
/// including the next segment and the direction of travel on that segment.
/// </summary>
/// <param name="NextSegment">The next track segment in the path.</param>
/// <param name="NewDirection">The direction of travel on the NextSegment.</param>
public record NavigationResult(ITrackSegment NextSegment, TravelDirection NewDirection);
