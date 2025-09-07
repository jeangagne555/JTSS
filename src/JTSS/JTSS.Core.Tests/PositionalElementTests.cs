using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;

namespace JTSS.Core.Tests;

// A minimal, concrete implementation of PositionalElement for testing purposes ONLY.
// It lives inside the test file and is not part of the main project.
internal class TestPositionalElement : PositionalElement
{
    public TestPositionalElement(string id, ITrackPosition position, string? name = null)
        : base(id, position, name)
    {
    }
}

// A minimal, concrete implementation of PositionalDirectionalElement for testing.
internal class TestPositionalDirectionalElement : PositionalDirectionalElement
{
    public TestPositionalDirectionalElement(string id, ITrackPosition position, TravelDirection direction, string? name = null)
        : base(id, position, direction, name)
    {
    }
}


public class PositionalElementTests
{
    private readonly ITrackSegment _segment;
    private readonly ITrackPosition _position;

    public PositionalElementTests()
    {
        _segment = new TrackSegment("seg-1", 100);
        _position = new TrackPosition(_segment, 50.0);
    }

    #region PositionalElement Base Tests

    [Fact]
    public void PositionalElement_Constructor_WithValidArgs_InitializesProperties()
    {
        // Act
        // We use our test-only class to test the abstract base class's logic.
        var element = new TestPositionalElement("detector-1", _position, "Main Detector");

        // Assert
        Assert.Equal("detector-1", element.Id);
        Assert.Equal("Main Detector", element.Name);
        Assert.Equal(_position, element.Position);
        Assert.Equal(_segment, element.Position.Segment);
        Assert.Equal(50.0, element.Position.DistanceFromLeftEnd);
    }

    [Fact]
    public void PositionalElement_Constructor_WithNullId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("id", () => new TestPositionalElement(null!, _position));
    }

    [Fact]
    public void PositionalElement_Constructor_WithNullPosition_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("position", () => new TestPositionalElement("detector-1", null!));
    }

    #endregion

    #region PositionalDirectionalElement Tests

    [Theory]
    [InlineData(TravelDirection.LeftToRight)]
    [InlineData(TravelDirection.RightToLeft)]
    public void PositionalDirectionalElement_Constructor_SetsAllPropertiesCorrectly(TravelDirection direction)
    {
        // Act
        var element = new TestPositionalDirectionalElement("signal-1", _position, direction, "Signal 1A");

        // Assert
        // Verify properties from the base class are still set correctly
        Assert.Equal("signal-1", element.Id);
        Assert.Equal("Signal 1A", element.Name);
        Assert.Equal(_position, element.Position);

        // Verify the new property from the derived abstract class
        Assert.Equal(direction, element.Direction);
    }

    #endregion
}