using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests;

public class TrackPathTests
{
    private readonly ITrackNetwork _network;
    private readonly ITrackNavigator _navigator;

    public TrackPathTests()
    {
        _network = new TrackNetwork();
        _navigator = new TrackNavigator();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidPath_InitializesPropertiesCorrectlyAndLengthIsConsistent()
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        double expectedLength = (100.0 - 20.0) + 80.0;

        var path = new TrackPath(startPos, endPos, _navigator);

        Assert.Equal(startPos, path.StartPosition);
        Assert.Equal(endPos, path.EndPosition);
        Assert.Equal(expectedLength, path.Length, 1);

        var calculatedDistance = _navigator.GetDistanceBetween(path.StartPosition, path.EndPosition);
        Assert.NotNull(calculatedDistance);
        Assert.Equal(path.Length, calculatedDistance.Value, 1);
    }

    [Fact]
    public void Constructor_WithNoValidPath_ThrowsArgumentException()
    {
        var (segA, _, segC, sw1) = BuildWyeLayout();
        sw1.State = SwitchState.Normal;

        var startPos = new TrackPosition(segA, 50.0);
        var endPos = new TrackPosition(segC, 50.0);

        var ex = Assert.Throws<ArgumentException>(() => new TrackPath(startPos, endPos, _navigator));
        Assert.Contains("No valid path exists", ex.Message);
    }

    #endregion

    #region Movement Tests

    // --- START OF FIX ---
    // The test signature now includes parameters for the initial start and end distances.
    [Theory]
    [MemberData(nameof(StraightPathMoveData))]
    public void Move_OnStraightTrack_MovesEndpointsCorrectlyAndMaintainsLengthConsistency(
        double initialStartDist, double initialEndDist,
        PathDirection direction, double moveDistance,
        string expectedStartSegId, double expectedStartDist,
        string expectedEndSegId, double expectedEndDist)
    {
        // Arrange
        var (segA, segB) = BuildStraightLayout();
        // The path is now created using the parameterized start and end positions.
        var startPos = new TrackPosition(segA, initialStartDist);
        var endPos = new TrackPosition(segB, initialEndDist);
        var path = new TrackPath(startPos, endPos, _navigator);

        // Act
        var movedPath = path.Move(direction, moveDistance);

        // Assert
        Assert.NotNull(movedPath);
        Assert.Equal(expectedStartSegId, movedPath.StartPosition.Segment.Id);
        Assert.Equal(expectedStartDist, movedPath.StartPosition.DistanceFromLeftEnd, 1);
        Assert.Equal(expectedEndSegId, movedPath.EndPosition.Segment.Id);
        Assert.Equal(expectedEndDist, movedPath.EndPosition.DistanceFromLeftEnd, 1);

        Assert.Equal(path.Length, movedPath.Length, 1);

        var calculatedDistance = _navigator.GetDistanceBetween(movedPath.StartPosition, movedPath.EndPosition);
        Assert.NotNull(calculatedDistance);
        Assert.Equal(movedPath.Length, calculatedDistance.Value, 1);
    }

    // The test data now includes the initial start and end distances for each scenario.
    public static IEnumerable<object[]> StraightPathMoveData =>
        new List<object[]>
        {
            // Case 1: Simple forward move
            // Start(A@20), End(B@80) -> Move Fwd 30m -> New Start(A@50), New End(B@110)
            new object[] { 20.0, 80.0, PathDirection.Forward, 30.0, "seg-A", 50.0, "seg-B", 110.0 },
            
            // Case 2: Simple backward move
            // Start(A@20), End(B@80) -> Move Bwd 15m -> New Start(A@5), New End(B@65)
            new object[] { 20.0, 80.0, PathDirection.Backward, 15.0, "seg-A", 5.0, "seg-B", 65.0 },
            
            // Case 3: THE FAILING CASE, NOW CORRECTED
            // Start(A@80), End(B@80) -> Move Fwd 40m -> New Start(B@20), New End(B@120)
            new object[] { 80.0, 80.0, PathDirection.Forward, 40.0, "seg-B", 20.0, "seg-B", 120.0 },
        };
    // --- END OF FIX ---

    [Theory]
    [MemberData(nameof(WyePathMoveData))]
    public void Move_OnWyeTrack_MaintainsLengthConsistency(
        PathDirection direction, double moveDistance,
        string expectedStartSegId, double expectedStartDist,
        string expectedEndSegId, double expectedEndDist)
    {
        var (segA, _, segC, _) = BuildWyeLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segC, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        var movedPath = path.Move(direction, moveDistance);

        Assert.NotNull(movedPath);
        Assert.Equal(expectedStartSegId, movedPath.StartPosition.Segment.Id);
        Assert.Equal(expectedStartDist, movedPath.StartPosition.DistanceFromLeftEnd, 1);
        Assert.Equal(expectedEndSegId, movedPath.EndPosition.Segment.Id);
        Assert.Equal(expectedEndDist, movedPath.EndPosition.DistanceFromLeftEnd, 1);

        Assert.Equal(path.Length, movedPath.Length, 1);

        var calculatedDistance = _navigator.GetDistanceBetween(movedPath.StartPosition, movedPath.EndPosition);
        Assert.NotNull(calculatedDistance);
        Assert.Equal(movedPath.Length, calculatedDistance.Value, 1);
    }

    public static IEnumerable<object[]> WyePathMoveData =>
        new List<object[]>
        {
            new object[] { PathDirection.Forward, 30.0, "seg-A", 50.0, "seg-C", 50.0 },
            new object[] { PathDirection.Backward, 10.0, "seg-A", 10.0, "seg-C", 90.0 },
        };

    [Fact]
    public void Move_MovesOffTrack_ReturnsNull()
    {
        var segA = _network.AddTrackSegment("seg-A", 100);
        var border = _network.AddBorderNode("border-1");
        border.Connect(new TrackConnection(segA, SegmentEnd.Left));

        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segA, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        var movedPath = path.Move(PathDirection.Backward, 30.0);

        Assert.Null(movedPath);
    }

    #endregion

    #region Merge Tests

    [Fact]
    public void Merge_WithValidAdjoiningPath_CreatesCorrectCombinedPath()
    {
        // Arrange
        // Create a layout: A(100) -- B(200) -- C(150)
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 200);
        var segC = _network.AddTrackSegment("seg-C", 150);
        var node1 = _network.AddStraightNode("node-1");
        var node2 = _network.AddStraightNode("node-2");
        node1.Connect(new TrackConnection(segA, SegmentEnd.Right), new TrackConnection(segB, SegmentEnd.Left));
        node2.Connect(new TrackConnection(segB, SegmentEnd.Right), new TrackConnection(segC, SegmentEnd.Left));

        // Path 1: From 50m on A to 70m on B
        var path1_start = new TrackPosition(segA, 50.0);
        var path1_end_and_path2_start = new TrackPosition(segB, 70.0);
        var path1 = new TrackPath(path1_start, path1_end_and_path2_start, _navigator);

        // Path 2: From 70m on B to 100m on C
        var path2_end = new TrackPosition(segC, 100.0);
        var path2 = new TrackPath(path1_end_and_path2_start, path2_end, _navigator);

        double expectedLength = path1.Length + path2.Length;

        // Act
        var mergedPath = path1.Merge(path2);

        // Assert
        Assert.NotNull(mergedPath);
        Assert.Equal(path1.StartPosition, mergedPath.StartPosition); // Start of the first path
        Assert.Equal(path2.EndPosition, mergedPath.EndPosition);     // End of the second path
        Assert.Equal(expectedLength, mergedPath.Length, 1);
    }

    [Fact]
    public void Merge_WithNonAdjoiningPath_ThrowsArgumentException()
    {
        // Arrange
        var (segA, segB) = BuildStraightLayout();

        var path1 = new TrackPath(new TrackPosition(segA, 10), new TrackPosition(segA, 50), _navigator);

        // This path starts at A@50.1, which is NOT where path1 ends.
        var path2 = new TrackPath(new TrackPosition(segA, 50.1), new TrackPosition(segA, 90), _navigator);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => path1.Merge(path2));
        Assert.Contains("Paths are not adjoining", ex.Message);
    }

    #endregion

    #region Helper Methods

    private (ITrackSegment, ITrackSegment) BuildStraightLayout()
    {
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 200);
        var node = _network.AddStraightNode("node-1");
        node.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segB, SegmentEnd.Left)
        );
        return (segA, segB);
    }

    private (ITrackSegment, ITrackSegment, ITrackSegment, ISwitchNode) BuildWyeLayout()
    {
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segS = _network.AddTrackSegment("seg-S", 200); // Straight
        var segC = _network.AddTrackSegment("seg-C", 150); // Curve
        var sw1 = _network.AddSwitchNode("sw-1");
        sw1.State = SwitchState.Reversed;
        sw1.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segS, SegmentEnd.Left),
            new TrackConnection(segC, SegmentEnd.Right)
        );
        return (segA, segS, segC, sw1);
    }

    #endregion
}
