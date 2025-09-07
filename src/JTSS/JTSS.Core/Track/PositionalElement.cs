using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Track;

/// <summary>
/// An abstract base class for track network elements that have a specific, fixed position on a track segment.
/// </summary>
public abstract class PositionalElement : IPositionalElement
{
    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc/>
    public ITrackPosition Position { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionalElement"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this element.</param>
    /// <param name="position">The precise position of this element.</param>
    /// <param name="name">An optional, user-friendly name for this element.</param>
    protected PositionalElement(string id, ITrackPosition position, string? name = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(position);

        Id = id;
        Position = position;
        Name = name;
    }
}