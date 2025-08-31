using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests;

public class BorderNodeTests
{
    [Fact]
    public void Connect_WithOneSegment_SucceedsAndSetsSegmentEndpoint()
    {
        // Arrange
        var node = new BorderNode("border-1");
        var segment = new TrackSegment("seg-1", 100);
        var connection = new TrackConnection(segment, SegmentEnd.Left);

        // Act
        node.Connect(connection);

        // Assert
        Assert.Single(node.Connections);
        Assert.Equal(segment, node.Connections[0]);
        Assert.Equal(node, segment.LeftEndNode);
    }

    [Fact]
    public void Connect_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var node = new BorderNode("border-1");
        var seg1 = new TrackSegment("seg-1", 100);
        var seg2 = new TrackSegment("seg-2", 100);
        node.Connect(new TrackConnection(seg1, SegmentEnd.Left));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => node.Connect(new TrackConnection(seg2, SegmentEnd.Left)));
    }

    [Fact]
    public void GetValidPaths_Always_ReturnsEmpty()
    {
        // Arrange
        var node = new BorderNode("border-1");
        var segment = new TrackSegment("seg-1", 100);
        node.Connect(new TrackConnection(segment, SegmentEnd.Left));

        // Act
        var paths = node.GetValidPaths(segment);

        // Assert
        Assert.Empty(paths);
    }
}
