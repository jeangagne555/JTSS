using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Tests;

public class CrossingNodeTests
{
    private readonly CrossingNode _node;
    private readonly Dictionary<string, ITrackSegment> _segments;

    public CrossingNodeTests()
    {
        // Arrange - setup is the same for all tests, so we do it in the constructor
        _node = new CrossingNode("cross-1");
        var line1a = new TrackSegment("l1a", 100);
        var line1b = new TrackSegment("l1b", 100);
        var line2a = new TrackSegment("l2a", 100);
        var line2b = new TrackSegment("l2b", 100);

        _segments = new Dictionary<string, ITrackSegment>
        {
            { "l1a", line1a }, { "l1b", line1b }, { "l2a", line2a }, { "l2b", line2b }
        };

        _node.Connect(
            new TrackConnection(line1a, SegmentEnd.Right),
            new TrackConnection(line1b, SegmentEnd.Left),
            new TrackConnection(line2a, SegmentEnd.Right),
            new TrackConnection(line2b, SegmentEnd.Left)
        );
    }

    [Theory]
    [InlineData("l1a", "l1b")]
    [InlineData("l1b", "l1a")]
    [InlineData("l2a", "l2b")]
    [InlineData("l2b", "l2a")]
    public void GetValidPaths_FromAnySegment_ReturnsCorrectOppositeSegment(string entryId, string expectedExitId)
    {
        // Arrange
        var entrySegment = _segments[entryId];
        var expectedExitSegment = _segments[expectedExitId];

        // Act
        var paths = _node.GetValidPaths(entrySegment).ToList();

        // Assert
        var actualExitSegment = Assert.Single(paths);
        Assert.Equal(expectedExitSegment, actualExitSegment);
    }
}