using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Tests.Classes;

// Test-only stub class
internal class TestPositionalElement : PositionalElement
{
    public TestPositionalElement(string id, ITrackPosition position, string? name = null)
        : base(id, position, name) { }
}

internal class TestPositionalDirectionalElement : PositionalDirectionalElement
{
    public TestPositionalDirectionalElement(string id, ITrackPosition position, TravelDirection direction, string? name = null)
        : base(id, position, direction, name)
    {
    }
}
