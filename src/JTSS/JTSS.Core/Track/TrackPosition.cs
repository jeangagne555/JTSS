using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Track;

/// <summary>
/// Represents a precise, immutable position on a specific track segment.
/// </summary>
public record TrackPosition : ITrackPosition
{
    /// <inheritdoc/>
    public ITrackSegment Segment { get; }

    /// <inheritdoc/>
    public double DistanceFromLeftEnd { get; }

    /// <summary>
    /// Initializes a new instance of the TrackPosition class.
    /// </summary>
    /// <param name="segment">The track segment on which the position exists.</param>
    /// <param name="distanceFromLeftEnd">The distance in meters from the left end of the segment.</param>
    /// <exception cref="ArgumentNullException">Thrown if the segment is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the distance is outside the bounds of the segment's length.</exception>
    public TrackPosition(ITrackSegment segment, double distanceFromLeftEnd)
    {
        ArgumentNullException.ThrowIfNull(segment);

        if (distanceFromLeftEnd < 0 || distanceFromLeftEnd > segment.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceFromLeftEnd),
                $"Distance must be between 0 and the segment length ({segment.Length}m). Provided value was {distanceFromLeftEnd}m.");
        }

        Segment = segment;
        DistanceFromLeftEnd = distanceFromLeftEnd;
    }
}