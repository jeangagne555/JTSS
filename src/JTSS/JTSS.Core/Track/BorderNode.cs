using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track;

/// <summary>
/// Represents a border node, which acts as a terminus for a track line within the simulation.
/// </summary>
public class BorderNode : IBorderNode
{
    public string Id { get; }
    public string? Name { get; set; }
    private readonly List<ITrackSegment> _connections = new(1);
    public IReadOnlyList<ITrackSegment> Connections => _connections;

    public BorderNode(string id, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = name;
    }

    // ... rest of the file is unchanged
    /// <inheritdoc/>
    public void Connect(TrackConnection connection)
    {
        if (_connections.Any())
        {
            throw new InvalidOperationException($"Border node '{Id}' can only have one connection and is already connected to segment '{_connections.First().Id}'.");
        }

        ArgumentNullException.ThrowIfNull(connection);

        _connections.Add(connection.Segment);
        connection.Segment.SetNodeAt(connection.End, this);
    }

    /// <summary>
    /// Returns an empty collection, as a train arriving at a border node has left the network.
    /// </summary>
    /// <param name="fromSegment">The segment the train is arriving on.</param>
    /// <returns>An empty enumerable, always.</returns>
    public IEnumerable<ITrackSegment> GetValidPaths(ITrackSegment fromSegment)
    {
        // A border node is the end of the line. There are no valid paths from here.
        return Enumerable.Empty<ITrackSegment>();
    }
}