using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Track;

/// <summary>
/// Manages the collection of all track segments and nodes.
/// </summary>
public class TrackNetwork : ITrackNetwork
{
    private readonly Dictionary<string, ITrackNetworkElement> _elements = new();

    /// <summary>
    /// Provides an efficient spatial lookup for finding elements by the segment they reside on.
    /// </summary>
    private readonly Dictionary<string, List<IPositionalElement>> _elementsBySegment = new();

    public ITrackNetworkElement? GetElementById(string id)
    {
        _elements.TryGetValue(id, out var element);
        return element;
    }

    public ITrackSegment? GetSegmentById(string id)
    {
        return GetElementById(id) as ITrackSegment;
    }

    public ITrackNode? GetNodeById(string id)
    {
        return GetElementById(id) as ITrackNode;
    }

    public IZone? GetZoneById(string id)
    {
        return GetElementById(id) as IZone;
    }

    public ITrackSegment AddTrackSegment(string id, double length, string? name = null)
    {
        var segment = new TrackSegment(id, length, name);
        RegisterElement(segment);
        return segment;
    }

    public IStraightNode AddStraightNode(string id, string? name = null)
    {
        var node = new StraightNode(id, name);
        RegisterElement(node);
        return node;
    }

    public ISwitchNode AddSwitchNode(string id, string? name = null)
    {
        var node = new SwitchNode(id, name);
        RegisterElement(node);
        return node;
    }

    public ICrossingNode AddCrossingNode(string id, string? name = null)
    {
        var node = new CrossingNode(id, name);
        RegisterElement(node);
        return node;
    }

    public IBorderNode AddBorderNode(string id, string? name = null)
    {
        var node = new BorderNode(id, name);
        RegisterElement(node);
        return node;
    }

    public IZone AddZone(string id, string? name = null)
    {
        var zone = new Zone(id, name);
        RegisterElement(zone);
        return zone;
    }

    private void RegisterElement(ITrackNetworkElement element)
    {
        if (!_elements.TryAdd(element.Id, element))
        {
            throw new ArgumentException($"An element with ID '{element.Id}' already exists in the network.");
        }

        // If the new element is a positional element, also add it to our spatial lookup.
        if (element is IPositionalElement positionalElement)
        {
            var segmentId = positionalElement.Position.Segment.Id;

            // If this is the first element being added to this segment, create the list.
            if (!_elementsBySegment.TryGetValue(segmentId, out var elementList))
            {
                elementList = new List<IPositionalElement>();
                _elementsBySegment[segmentId] = elementList;
            }

            elementList.Add(positionalElement);
        }
    }

    /// <summary>
    /// Gets all positional elements located on a specific track segment.
    /// This is an internal-facing method for the navigator to use.
    /// </summary>
    /// <param name="segmentId">The ID of the track segment.</param>
    /// <returns>An enumerable of positional elements on that segment.</returns>
    internal IEnumerable<IPositionalElement> GetElementsBySegmentId(string segmentId)
    {
        if (_elementsBySegment.TryGetValue(segmentId, out var elementList))
        {
            return elementList;
        }
        return Enumerable.Empty<IPositionalElement>();
    }
}