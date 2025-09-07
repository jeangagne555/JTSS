using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Track;

/// <summary>
/// An abstract base class for positional elements that also have a direction of relevance, such as signals.
/// </summary>
public abstract class PositionalDirectionalElement : PositionalElement, IPositionalDirectionalElement
{
    /// <inheritdoc/>
    public TravelDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionalDirectionalElement"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this element.</param>
    /// <param name="position">The precise position of this element.</param>
    /// <param name="direction">The direction of relevance for this element.</param>
    /// <param name="name">An optional, user-friendly name for this element.</param>
    protected PositionalDirectionalElement(string id, ITrackPosition position, TravelDirection direction, string? name = null)
        : base(id, position, name)
    {
        Direction = direction;
    }
}
