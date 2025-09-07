using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests;

public class ZoneTests
{
    private readonly ITrackNetwork _network;
    private readonly ITrackNavigator _navigator;
    private readonly ITrackSegment _segA;
    private readonly ITrackSegment _segB;

    // The constructor acts as our setup method, providing a consistent track layout for each test.
    public ZoneTests()
    {
        _network = new TrackNetwork();
        _navigator = new TrackNavigator();

        // Layout: seg-A (100m) -- node -- seg-B (100m)
        _segA = _network.AddTrackSegment("seg-A", 100);
        _segB = _network.AddTrackSegment("seg-B", 100);
        var node = _network.AddStraightNode("node-1");
        node.Connect(
            new TrackConnection(_segA, SegmentEnd.Right),
            new TrackConnection(_segB, SegmentEnd.Left)
        );
    }

    [Fact]
    public void Constructor_WithValidId_InitializesCorrectly()
    {
        // Arrange
        var zone = new Zone("zone-1");

        // Assert
        Assert.Equal("zone-1", zone.Id);
        Assert.Null(zone.Name);
        Assert.NotNull(zone.TrackPaths);
        Assert.Empty(zone.TrackPaths);
    }

    [Fact]
    public void Constructor_WithName_SetsNameProperty()
    {
        // Arrange
        var zone = new Zone("zone-1", "Station Platform");

        // Assert
        Assert.Equal("zone-1", zone.Id);
        Assert.Equal("Station Platform", zone.Name);
    }

    [Fact]
    public void Constructor_WithNullId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Zone(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyOrWhitespaceId_ThrowsArgumentException(string invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Zone(invalidId));
    }

    // ... rest of the file is unchanged
    [Fact]
    public void AddPath_AddsPathToCollection()
    {
        // Arrange
        var zone = new Zone("zone-1");
        var path = new TrackPath(new TrackPosition(_segA, 10), new TrackPosition(_segA, 90), _navigator);

        // Act
        zone.AddPath(path);

        // Assert
        Assert.Single(zone.TrackPaths);
        Assert.Equal(path, zone.TrackPaths[0]);
    }

    [Fact]
    public void AddPath_AddingSamePathInstanceTwice_DoesNotCreateDuplicate()
    {
        // Arrange
        var zone = new Zone("zone-1");
        var path = new TrackPath(new TrackPosition(_segA, 10), new TrackPosition(_segA, 90), _navigator);

        // Act
        zone.AddPath(path);
        zone.AddPath(path); // Add the same instance again

        // Assert
        Assert.Single(zone.TrackPaths);
    }

    [Fact]
    public void IntersectsWith_WhenPathOverlapsZonePath_ReturnsTrue()
    {
        // Arrange
        var zone = new Zone("zone-1");
        // Zone covers the end of seg-A and the start of seg-B
        var zonePath = new TrackPath(new TrackPosition(_segA, 80), new TrackPosition(_segB, 20), _navigator);
        zone.AddPath(zonePath);

        // Test path starts within the zone on seg-A
        var intersectingPath = new TrackPath(new TrackPosition(_segA, 90), new TrackPosition(_segB, 50), _navigator);

        // Act
        var result = zone.IntersectsWith(intersectingPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenPathTouchesEndpointOfZonePath_ReturnsTrue()
    {
        // Arrange
        var zone = new Zone("zone-1");
        var zonePath = new TrackPath(new TrackPosition(_segA, 20), new TrackPosition(_segA, 80), _navigator);
        zone.AddPath(zonePath);

        // Test path starts exactly where the zone path ends
        var touchingPath = new TrackPath(new TrackPosition(_segA, 80), new TrackPosition(_segB, 40), _navigator);

        // Act
        var result = zone.IntersectsWith(touchingPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenPathIsDisjoint_ReturnsFalse()
    {
        // Arrange
        var zone = new Zone("zone-1");
        var zonePath = new TrackPath(new TrackPosition(_segA, 10), new TrackPosition(_segA, 40), _navigator);
        zone.AddPath(zonePath);

        // Test path is far away on another segment
        var disjointPath = new TrackPath(new TrackPosition(_segB, 50), new TrackPosition(_segB, 90), _navigator);

        // Act
        var result = zone.IntersectsWith(disjointPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IntersectsWith_WhenZoneHasMultiplePaths_FindsIntersection()
    {
        // Arrange
        var zone = new Zone("zone-1");
        var zonePath1 = new TrackPath(new TrackPosition(_segA, 10), new TrackPosition(_segA, 30), _navigator);
        var zonePath2 = new TrackPath(new TrackPosition(_segB, 70), new TrackPosition(_segB, 90), _navigator);
        zone.AddPath(zonePath1);
        zone.AddPath(zonePath2);

        // This path intersects with the second path in the zone
        var intersectingPath = new TrackPath(new TrackPosition(_segB, 60), new TrackPosition(_segB, 80), _navigator);

        // Act
        var result = zone.IntersectsWith(intersectingPath);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenZoneIsEmpty_ReturnsFalse()
    {
        // Arrange
        var zone = new Zone("zone-1"); // An empty zone
        var path = new TrackPath(new TrackPosition(_segA, 10), new TrackPosition(_segA, 90), _navigator);

        // Act
        var result = zone.IntersectsWith(path);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TrackNetwork_AddZone_CanRetrieveZoneById()
    {
        // Arrange
        var network = new TrackNetwork();

        // Act
        var addedZone = network.AddZone("my-zone", "Yard Lead");
        var retrievedZone = network.GetZoneById("my-zone");
        var retrievedElement = network.GetElementById("my-zone");

        // Assert
        Assert.NotNull(addedZone);
        Assert.Equal(addedZone, retrievedZone);
        Assert.Equal(addedZone, retrievedElement);
        Assert.IsAssignableFrom<IZone>(retrievedElement);
        Assert.Equal("Yard Lead", retrievedZone?.Name);
    }

    [Fact]
    public void TrackNetwork_AddZone_WithDuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var network = new TrackNetwork();
        network.AddZone("duplicate-zone");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => network.AddZone("duplicate-zone"));
    }
}