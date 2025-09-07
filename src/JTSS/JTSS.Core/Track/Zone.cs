using JTSS.Core.Track.Interfaces;
using System.Linq;

namespace JTSS.Core.Track;

/// <summary>
/// A concrete implementation of a zone, representing a collection of track paths.
/// </summary>
public class Zone : IZone
{
    public string Id { get; }
    public string? Name { get; set; }

    private readonly List<ITrackPath> _trackPaths = new();
    public IReadOnlyList<ITrackPath> TrackPaths => _trackPaths;

    /// <summary>
    /// Initializes a new instance of the Zone class.
    /// </summary>
    /// <param name="id">The unique identifier for the zone.</param>
    /// <param name="name">An optional, user-friendly name for the zone.</param>
    public Zone(string id, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = name;
    }

    // ... rest of the file is unchanged
    /// <inheritdoc/>
    public void AddPath(ITrackPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        // Avoid adding duplicate path instances
        if (!_trackPaths.Contains(path))
        {
            _trackPaths.Add(path);
        }
    }

    /// <inheritdoc/>
    public bool IntersectsWith(ITrackPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        // The given path intersects with the zone if it intersects with ANY of the paths
        // that make up the zone. Using LINQ's Any() is efficient as it will stop
        // checking as soon as the first intersection is found.
        return _trackPaths.Any(zonePath => zonePath.IntersectsWith(path));
    }
}