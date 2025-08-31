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

public class TrackNavigatorTests
{
    private readonly ITrackNetwork _network;
    private readonly ITrackNavigator _navigator;

    // The constructor is our "Setup" method. It runs before every single test.
    public TrackNavigatorTests()
    {
        _network = new TrackNetwork();
        _navigator = new TrackNavigator();
    }

    [Fact]
    public void MovePosition_StaysOnSameSegment_CalculatesCorrectly()
    {
        // Arrange (Set up the world for this specific test)
        var segA = _network.AddTrackSegment("seg-A", 100);
        var startPos = new TrackPosition(segA, 20.0);

        // Act (Perform the action we are testing)
        var endPos = _navigator.MovePosition(startPos, TravelDirection.LeftToRight, 50.0);

        // Assert (Verify the outcome is what we expect)
        Assert.NotNull(endPos);
        Assert.Equal(segA.Id, endPos.Segment.Id);
        Assert.Equal(70.0, endPos.DistanceFromLeftEnd);
    }

    [Theory]
    [InlineData(80.0, 70.0, "seg-B", 50.0)] // Crosses from seg-A to seg-B
    [InlineData(10.0, 90.0, "seg-B", 0.0)]  // Lands exactly on the node, which is the start of seg-B
    [InlineData(95.0, 105.0, "seg-C", 100.0)] // Crosses from seg-A to seg-C via the switch
    public void MovePosition_CrossesNode_CalculatesCorrectNewPositionAndSegment(
        double startDist, double moveDist, string expectedSegId, double expectedEndDist)
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 200);
        var segC = _network.AddTrackSegment("seg-C", 150);
        var sw1 = _network.AddSwitchNode("sw-1");
        sw1.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segB, SegmentEnd.Left),
            new TrackConnection(segC, SegmentEnd.Left)
        );

        // For the test case that goes to segC, we need to flip the switch
        if (expectedSegId == "seg-C")
        {
            sw1.State = SwitchState.Reversed;
        }

        var startPos = new TrackPosition(segA, startDist);

        // Act
        var endPos = _navigator.MovePosition(startPos, TravelDirection.LeftToRight, moveDist);

        // Assert
        Assert.NotNull(endPos);
        Assert.Equal(expectedSegId, endPos.Segment.Id);
        Assert.Equal(expectedEndDist, endPos.DistanceFromLeftEnd, 1); // Using a precision of 1 decimal place for float comparison
    }

    [Fact]
    public void GetDistanceBetween_OnSameSegment_IsCorrect()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var pos1 = new TrackPosition(segA, 15.0);
        var pos2 = new TrackPosition(segA, 85.0);

        // Act
        var distance = _navigator.GetDistanceBetween(pos1, pos2);

        // Assert
        Assert.NotNull(distance);
        Assert.Equal(70.0, distance.Value);
    }

    #region ArePositionsApproximatelyEqual Tests

    [Fact]
    public void ArePositionsApproximatelyEqual_WithSameSegmentAndWithinTolerance_ReturnsTrue()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var pos1 = new TrackPosition(segA, 50.0);
        var pos2 = new TrackPosition(segA, 50.0 + (TrackPrecision.Tolerance / 2)); // e.g. 50.05

        // Act
        var result = _navigator.ArePositionsApproximatelyEqual(pos1, pos2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ArePositionsApproximatelyEqual_WithSameSegmentAndOutsideTolerance_ReturnsFalse()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var pos1 = new TrackPosition(segA, 50.0);
        var pos2 = new TrackPosition(segA, 50.0 + TrackPrecision.Tolerance + 0.01); // e.g. 50.11

        // Act
        var result = _navigator.ArePositionsApproximatelyEqual(pos1, pos2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ArePositionsApproximatelyEqual_AcrossNodeAndWithinTolerance_ReturnsTrue()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 100);
        var node = _network.AddStraightNode("node-1");
        node.Connect(new TrackConnection(segA, SegmentEnd.Right), new TrackConnection(segB, SegmentEnd.Left));

        // Position A is on seg-A, just before the end.
        var posA = new TrackPosition(segA, 100.0 - (TrackPrecision.Tolerance / 2)); // e.g. 99.95
        // Position B is on seg-B, just after the start.
        var posB = new TrackPosition(segB, 0.0 + (TrackPrecision.Tolerance / 2));   // e.g. 0.05

        // Act
        var result = _navigator.ArePositionsApproximatelyEqual(posA, posB);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ArePositionsApproximatelyEqual_AcrossNodeAndOutsideTolerance_ReturnsFalse()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 100);
        var node = _network.AddStraightNode("node-1");
        node.Connect(new TrackConnection(segA, SegmentEnd.Right), new TrackConnection(segB, SegmentEnd.Left));

        var posA = new TrackPosition(segA, 99.0); // 1.0m from end
        var posB = new TrackPosition(segB, 1.0);  // 1.0m from start

        // Act
        var result = _navigator.ArePositionsApproximatelyEqual(posA, posB);

        // Assert
        Assert.False(result);
    }

    #endregion
}
