using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests.Track;

public class StraightNodeTests
{
    [Fact]
    public void GetValidPaths_FromFirstSegment_ReturnsSecondSegment()
    {
        // Arrange
        var node = new StraightNode("node-1");
        var segA = new TrackSegment("seg-A", 100);
        var segB = new TrackSegment("seg-B", 100);
        node.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segB, SegmentEnd.Left)
        );

        // Act
        var path = node.GetValidPaths(segA);

        // Assert
        var resultSegment = Assert.Single(path);
        Assert.Equal(segB, resultSegment);
    }

    [Fact]
    public void GetValidPaths_FromSecondSegment_ReturnsFirstSegment()
    {
        // Arrange
        var node = new StraightNode("node-1");
        var segA = new TrackSegment("seg-A", 100);
        var segB = new TrackSegment("seg-B", 100);
        node.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segB, SegmentEnd.Left)
        );

        // Act
        var path = node.GetValidPaths(segB).ToList();

        // Assert
        var resultSegment = Assert.Single(path);
        Assert.Equal(segA, resultSegment);
    }
}
