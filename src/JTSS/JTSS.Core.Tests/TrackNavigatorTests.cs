using JTSS.Core.Tests.Classes;
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
    private readonly TrackNetwork _network;
    private readonly ITrackNavigator _navigator;

    // The constructor is our "Setup" method. It runs before every single test.
    public TrackNavigatorTests()
    {
        _network = new TrackNetwork();
        _navigator = new TrackNavigator(_network);
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

    #region GetElementsInPath Tests

    [Fact]
    public void GetElementsInPath_FindsElementsOnSingleSegmentPath()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var path = new TrackPath(new TrackPosition(segA, 10), new TrackPosition(segA, 90), _navigator);

        // Add elements to the network. The network will handle storing them.
        var elem1 = new TestPositionalElement("det-1", new TrackPosition(segA, 5.0));  // Before path
        var elem2 = new TestPositionalElement("det-2", new TrackPosition(segA, 25.0)); // In path
        var elem3 = new TestPositionalElement("det-3", new TrackPosition(segA, 75.0)); // In path
        var elem4 = new TestPositionalElement("det-4", new TrackPosition(segA, 95.0)); // After path
        _network.RegisterElementForTesting(elem1); // Helper method to add pre-made elements
        _network.RegisterElementForTesting(elem2);
        _network.RegisterElementForTesting(elem3);
        _network.RegisterElementForTesting(elem4);

        // Act
        var elements = _navigator.GetElementsInPath(path).ToList();

        // Assert
        Assert.Equal(2, elements.Count);
        Assert.Equal("det-2", elements[0].Element.Id);
        Assert.Equal(15.0, elements[0].DistanceAlongPath, 1); // 25.0 (pos) - 10.0 (start) = 15.0
        Assert.Equal("det-3", elements[1].Element.Id);
        Assert.Equal(65.0, elements[1].DistanceAlongPath, 1); // 75.0 (pos) - 10.0 (start) = 65.0
    }

    [Fact]
    public void GetElementsInPath_FindsElementsAcrossMultipleSegmentsAndOrdersThem()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 100);
        var node = _network.AddStraightNode("node-1");
        node.Connect(new(segA, SegmentEnd.Right), new(segB, SegmentEnd.Left));

        var path = new TrackPath(new TrackPosition(segA, 50), new TrackPosition(segB, 50), _navigator);

        var elem1 = new TestPositionalElement("det-A", new TrackPosition(segA, 75.0));
        var elem2 = new TestPositionalElement("det-B", new TrackPosition(segB, 25.0));
        _network.RegisterElementForTesting(elem1);
        _network.RegisterElementForTesting(elem2);

        // Act
        var elements = _navigator.GetElementsInPath(path).ToList();

        // Assert
        Assert.Equal(2, elements.Count);

        // First element is on segA. Distance = 75 - 50 = 25
        Assert.Equal("det-A", elements[0].Element.Id);
        Assert.Equal(25.0, elements[0].DistanceAlongPath, 1);

        // Second element is on segB. Distance = (100 - 50 on segA) + 25 on segB = 75
        Assert.Equal("det-B", elements[1].Element.Id);
        Assert.Equal(75.0, elements[1].DistanceAlongPath, 1);
    }

    #endregion
}

// Add a helper to TrackNetwork to allow adding already-created elements for testing.
// This is a common pattern for testability.
public static class TrackNetworkTestExtensions
{
    public static void RegisterElementForTesting(this TrackNetwork network, ITrackNetworkElement element)
    {
        // This uses reflection to call the private method, which is acceptable for test code.
        var method = typeof(TrackNetwork).GetMethod("RegisterElement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(network, new object[] { element });
    }
}
