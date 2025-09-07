using JTSS.Core.Track.Enums;

namespace JTSS.Core.Track.Interfaces;

/// <summary>
/// Defines a positional element that also has a direction of relevance.
/// This is for elements, like signals, that are faced by trains traveling in a specific direction.
/// </summary>
public interface IPositionalDirectionalElement : IPositionalElement
{
    /// <summary>
    /// The direction of travel for a train that would be considered "facing" this element.
    /// </summary>
    TravelDirection Direction { get; }
}
