using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests.Track;

public class TrackPathTests
{
    private readonly TrackNetwork _network;
    private readonly ITrackNavigator _navigator;

    public TrackPathTests()
    {
        _network = new TrackNetwork();
        _navigator = new TrackNavigator(_network);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidPath_InitializesPropertiesCorrectlyAndLengthIsConsistent()
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        double expectedLength = 100.0 - 20.0 + 80.0;

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

    [Theory]
    [MemberData(nameof(StraightPathMoveData))]
    public void Move_OnStraightTrack_MovesEndpointsCorrectlyAndMaintainsLengthConsistency(
        double initialStartDist, double initialEndDist,
        PathDirection direction, double moveDistance,
        string expectedStartSegId, double expectedStartDist,
        string expectedEndSegId, double expectedEndDist)
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, initialStartDist);
        var endPos = new TrackPosition(segB, initialEndDist);
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

    public static IEnumerable<object[]> StraightPathMoveData =>
        new List<object[]>
        {
            new object[] { 20.0, 80.0, PathDirection.Forward, 30.0, "seg-A", 50.0, "seg-B", 110.0 },
            new object[] { 20.0, 80.0, PathDirection.Backward, 15.0, "seg-A", 5.0, "seg-B", 65.0 },
            new object[] { 80.0, 80.0, PathDirection.Forward, 40.0, "seg-B", 20.0, "seg-B", 120.0 },
        };

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
            new object[] { PathDirection.Forward, 30.0, "wye-A", 50.0, "wye-C", 50.0 },
            new object[] { PathDirection.Backward, 10.0, "wye-A", 10.0, "wye-C", 90.0 },
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
    public void Merge_WithExactAdjoiningPath_CreatesCorrectCombinedPath()
    {
        var (segA, segB) = BuildStraightLayout();
        var path1_start = new TrackPosition(segA, 50.0);
        var common_pos = new TrackPosition(segB, 70.0);
        var path2_end = new TrackPosition(segB, 150.0);

        var path1 = new TrackPath(path1_start, common_pos, _navigator);
        var path2 = new TrackPath(common_pos, path2_end, _navigator);

        double expectedLength = path1.Length + path2.Length;

        var mergedPath = path1.Merge(path2);

        Assert.NotNull(mergedPath);
        Assert.Equal(path1.StartPosition, mergedPath.StartPosition);
        Assert.Equal(path2.EndPosition, mergedPath.EndPosition);
        Assert.Equal(expectedLength, mergedPath.Length, 1);
    }

    [Fact]
    public void Merge_WithAdjoiningPathWithinTolerance_Succeeds()
    {
        var (segA, _) = BuildStraightLayout();
        var path1_start = new TrackPosition(segA, 10.0);
        var path1_end = new TrackPosition(segA, 50.0);

        var path2_start = new TrackPosition(segA, 50.0 + TrackPrecision.Tolerance / 2);
        var path2_end = new TrackPosition(segA, 90.0);

        var path1 = new TrackPath(path1_start, path1_end, _navigator);
        var path2 = new TrackPath(path2_start, path2_end, _navigator);

        var mergedPath = path1.Merge(path2);

        Assert.NotNull(mergedPath);
        Assert.Equal(path1.StartPosition, mergedPath.StartPosition);
        Assert.Equal(path2.EndPosition, mergedPath.EndPosition);
    }

    [Fact]
    public void Merge_WithNonAdjoiningPathOutsideTolerance_ThrowsArgumentException()
    {
        var (segA, _) = BuildStraightLayout();
        var path1 = new TrackPath(new TrackPosition(segA, 10), new TrackPosition(segA, 50), _navigator);

        var path2_start_dist = 50.0 + TrackPrecision.Tolerance + 0.1;
        var path2 = new TrackPath(new TrackPosition(segA, path2_start_dist), new TrackPosition(segA, 90), _navigator);

        var ex = Assert.Throws<ArgumentException>(() => path1.Merge(path2));
        Assert.Contains("Paths are not adjoining", ex.Message);
    }

    #endregion

    #region Split Tests

    [Fact]
    public void Split_FromStart_CreatesTwoCorrectPaths()
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        double splitDistance = 50.0;

        var (first, second) = path.Split(splitDistance, SplitOrigin.FromStart);

        Assert.NotNull(first);
        Assert.NotNull(second);

        Assert.Equal(path.StartPosition, first.StartPosition);
        Assert.Equal(path.EndPosition, second.EndPosition);
        Assert.True(_navigator.ArePositionsApproximatelyEqual(first.EndPosition, second.StartPosition));

        Assert.Equal(splitDistance, first.Length, 1);
        Assert.Equal(path.Length - splitDistance, second.Length, 1);
    }

    [Fact]
    public void Split_FromEnd_CreatesTwoCorrectPaths()
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        double splitDistance = 60.0;

        var (first, second) = path.Split(splitDistance, SplitOrigin.FromEnd);

        Assert.NotNull(first);
        Assert.NotNull(second);

        Assert.Equal(path.StartPosition, first.StartPosition);
        Assert.Equal(path.EndPosition, second.EndPosition);
        Assert.True(_navigator.ArePositionsApproximatelyEqual(first.EndPosition, second.StartPosition));

        Assert.Equal(path.Length - splitDistance, first.Length, 1);
        Assert.Equal(splitDistance, second.Length, 1);
    }

    [Fact]
    public void Split_AtPointCrossingNode_Succeeds()
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        double splitDistance = 80.0;

        var (first, second) = path.Split(splitDistance, SplitOrigin.FromStart);

        var expectedSplitPosition = new TrackPosition(segA, 100.0);

        Assert.Equal(path.StartPosition, first.StartPosition);
        Assert.Equal(path.EndPosition, second.EndPosition);
        Assert.True(_navigator.ArePositionsApproximatelyEqual(first.EndPosition, expectedSplitPosition));
        Assert.True(_navigator.ArePositionsApproximatelyEqual(second.StartPosition, expectedSplitPosition));
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(161.0)]
    public void Split_WithInvalidDistance_ThrowsArgumentOutOfRangeException(double invalidDistance)
    {
        var (segA, segB) = BuildStraightLayout();
        var startPos = new TrackPosition(segA, 20.0);
        var endPos = new TrackPosition(segB, 80.0);
        var path = new TrackPath(startPos, endPos, _navigator);

        Assert.Throws<ArgumentOutOfRangeException>(() => path.Split(invalidDistance, SplitOrigin.FromStart));
    }

    #endregion

    #region IntersectsWith Tests

    [Fact]
    public void IntersectsWith_WhenPathsPartiallyOverlap_ReturnsTrue()
    {
        var (segA, segB) = BuildStraightLayout();
        var pathA = new TrackPath(new TrackPosition(segA, 20.0), new TrackPosition(segA, 80.0), _navigator);
        var pathB = new TrackPath(new TrackPosition(segA, 60.0), new TrackPosition(segB, 20.0), _navigator);

        bool result = pathA.IntersectsWith(pathB);

        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenOnePathContainsAnother_ReturnsTrue()
    {
        var (segA, segB) = BuildStraightLayout();
        var pathA = new TrackPath(new TrackPosition(segA, 80.0), new TrackPosition(segB, 120.0), _navigator);
        var pathB = new TrackPath(new TrackPosition(segB, 40.0), new TrackPosition(segB, 80.0), _navigator);

        bool result = pathA.IntersectsWith(pathB);

        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenPathsAreSeparate_ReturnsFalse()
    {
        var (segA, segB) = BuildStraightLayout();
        var pathA = new TrackPath(new TrackPosition(segA, 20.0), new TrackPosition(segA, 40.0), _navigator);
        var pathB = new TrackPath(new TrackPosition(segB, 80.0), new TrackPosition(segB, 120.0), _navigator);

        bool result = pathA.IntersectsWith(pathB);

        Assert.False(result);
    }

    [Fact]
    public void IntersectsWith_WhenPathsTouchAtEndpoint_ReturnsTrue()
    {
        var (segA, segB) = BuildStraightLayout();
        var pos1 = new TrackPosition(segA, 20.0);
        var commonPos = new TrackPosition(segA, 80.0);
        var pos3 = new TrackPosition(segB, 50.0);
        var pathA = new TrackPath(pos1, commonPos, _navigator);
        var pathB = new TrackPath(commonPos, pos3, _navigator);

        bool result = pathA.IntersectsWith(pathB);

        Assert.True(result);
    }

    [Fact]
    public void IntersectsWith_WhenPathsAreOnDisconnectedTracks_ReturnsFalse()
    {
        var (segA, _) = BuildStraightLayout();
        var pathA = new TrackPath(new TrackPosition(segA, 20.0), new TrackPosition(segA, 80.0), _navigator);

        var (wyeSegA, _, _, _) = BuildWyeLayout();
        var pathB = new TrackPath(new TrackPosition(wyeSegA, 10.0), new TrackPosition(wyeSegA, 90.0), _navigator);

        bool result = pathA.IntersectsWith(pathB);

        Assert.False(result);
    }

    #endregion

    #region IsOnSegment Tests

    [Fact]
    public void IsOnSegment_ForPathOnSingleSegment_ReturnsCorrectValues()
    {
        // Arrange
        var (segA, segB) = BuildStraightLayout();
        var path = new TrackPath(
            new TrackPosition(segA, 20.0),
            new TrackPosition(segA, 80.0),
            _navigator);

        // Act & Assert
        Assert.True(path.IsOnSegment(segA));
        Assert.False(path.IsOnSegment(segB));
        Assert.Single(path.CoveredSegments);
    }

    [Fact]
    public void IsOnSegment_ForPathSpanningMultipleSegments_ReturnsCorrectValuesForAllSegments()
    {
        // Arrange
        // Layout: A -- B -- C
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 100);
        var segC = _network.AddTrackSegment("seg-C", 100);
        var segD = _network.AddTrackSegment("seg-D", 100); // An unrelated segment
        var node1 = _network.AddStraightNode("node-1");
        var node2 = _network.AddStraightNode("node-2");
        node1.Connect(new TrackConnection(segA, SegmentEnd.Right), new TrackConnection(segB, SegmentEnd.Left));
        node2.Connect(new TrackConnection(segB, SegmentEnd.Right), new TrackConnection(segC, SegmentEnd.Left));

        // Path from middle of A to middle of C, crossing all of B.
        var path = new TrackPath(
            new TrackPosition(segA, 50.0),
            new TrackPosition(segC, 50.0),
            _navigator);

        // Act & Assert
        Assert.True(path.IsOnSegment(segA)); // The start segment
        Assert.True(path.IsOnSegment(segB)); // The middle segment
        Assert.True(path.IsOnSegment(segC)); // The end segment
        Assert.False(path.IsOnSegment(segD)); // The unrelated segment

        Assert.Equal(3, path.CoveredSegments.Count);
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

    // --- START OF FIX ---
    // The IDs in this method are now unique to prevent conflicts with other helper methods.
    private (ITrackSegment, ITrackSegment, ITrackSegment, ISwitchNode) BuildWyeLayout()
    {
        var segA = _network.AddTrackSegment("wye-A", 100);
        var segS = _network.AddTrackSegment("wye-S", 200);
        var segC = _network.AddTrackSegment("wye-C", 150);
        var sw1 = _network.AddSwitchNode("wye-sw-1");
        sw1.State = SwitchState.Reversed;
        sw1.Connect(
            new TrackConnection(segA, SegmentEnd.Right),
            new TrackConnection(segS, SegmentEnd.Left),
            new TrackConnection(segC, SegmentEnd.Right)
        );
        return (segA, segS, segC, sw1);
    }
    // --- END OF FIX ---

    #endregion
}