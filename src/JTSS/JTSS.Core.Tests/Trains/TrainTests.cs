using JTSS.Core.Simulator.Enums;
using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Tests.Classes;
using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Trains;
using Xunit;

namespace JTSS.Core.Tests.Trains;

// A simple test double for ISimulationState
internal class TestSimulationState : ISimulationState
{
    public TimeSpan CurrentTime { get; set; }
    public SimulationStatus Status { get; set; }
}

public class TrainTests
{
    // --- START OF CHANGE: Added Network and Navigator for constructor ---
    private readonly ISimulationState _simulationState;
    private readonly TrackNetwork _network;
    private readonly ITrackNavigator _navigator;

    public TrainTests()
    {
        _simulationState = new TestSimulationState();
        _network = new TrackNetwork();
        _navigator = new TrackNavigator(_network);
    }
    // --- END OF CHANGE ---

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidArgs_InitializesProperties()
    {
        // Arrange
        // --- START OF CHANGE: Added _navigator ---
        var train = new Train("T-01", 150.5, _simulationState, _navigator, "Freight 1");
        // --- END OF CHANGE ---

        // Assert
        Assert.Equal("T-01", train.Id);
        Assert.Equal("Freight 1", train.Name);
        Assert.Equal(150.5, train.Length);
        Assert.Null(train.Path);
        Assert.Null(train.Head);
        Assert.Null(train.Tail);
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullId_ThrowsArgumentNullException(string invalidId)
    {
        // Act & Assert
        // --- START OF CHANGE: Added _navigator ---
        Assert.Throws<ArgumentNullException>(() => new Train(invalidId, 150.0, _simulationState, _navigator));
        // --- END OF CHANGE ---
    }

    [Fact]
    public void Constructor_WithNullSimulationState_ThrowsArgumentNullException()
    {
        // Act & Assert
        // --- START OF CHANGE: Added _navigator ---
        Assert.Throws<ArgumentNullException>("simulationState", () => new Train("T-01", 150.0, null!, _navigator));
        // --- END OF CHANGE ---
    }

    // --- START OF NEW TEST ---
    [Fact]
    public void Constructor_WithNullNavigator_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("navigator", () => new Train("T-01", 150.0, _simulationState, null!));
    }
    // --- END OF NEW TEST ---

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidLength_ThrowsArgumentOutOfRangeException(double invalidLength)
    {
        // Act & Assert
        // --- START OF CHANGE: Added _navigator ---
        Assert.Throws<ArgumentOutOfRangeException>("length", () => new Train("T-01", invalidLength, _simulationState, _navigator));
        // --- END OF CHANGE ---
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyOrWhitespaceId_ThrowsArgumentException(string invalidId)
    {
        // Act & Assert
        // --- START OF CHANGE: Added _navigator ---
        Assert.Throws<ArgumentException>(() => new Train(invalidId, 150.0, _simulationState, _navigator));
        // --- END OF CHANGE ---
    }

    #endregion

    #region Update Method Tests

    [Fact]
    public void Update_DoesNotThrow()
    {
        // Arrange
        // --- START OF CHANGE: Added _navigator ---
        var train = new Train("T-02", 100.0, _simulationState, _navigator);
        // --- END OF CHANGE ---
        var timeSpan = TimeSpan.FromSeconds(1);

        // Act
        var exception = Record.Exception(() => train.Update(timeSpan));

        // Assert
        Assert.Null(exception);
    }

    #endregion

    // --- START OF NEW REGION: All placement tests are added here ---
    #region Placement Tests

    [Fact]
    public void Place_SuccessfulPlacement_SetsPathHeadAndTailCorrectly()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 200);
        var headPos = new TrackPosition(segA, 150.0);
        var train = new Train("T-01", 100.0, _simulationState, _navigator);

        // Act
        train.Place(headPos, TravelDirection.LeftToRight);

        // Assert
        Assert.NotNull(train.Path);
        Assert.Equal(headPos, train.Head);
        Assert.NotNull(train.Tail);
        Assert.Equal(segA.Id, train.Tail.Segment.Id);
        Assert.Equal(50.0, train.Tail.DistanceFromLeftEnd); // 150.0 - 100.0 = 50.0
        Assert.Equal(train.Length, train.Path.Length, 1);
    }

    [Fact]
    public void Place_SuccessfulPlacementAcrossNode_CalculatesTailCorrectly()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        var segB = _network.AddTrackSegment("seg-B", 100);
        _network.AddStraightNode("node-1").Connect(new(segB, SegmentEnd.Right), new(segA, SegmentEnd.Left));

        var headPos = new TrackPosition(segA, 50.0); // Head is 50m into segA
        var train = new Train("T-01", 80.0, _simulationState, _navigator); // 80m long

        // Act: Place the train facing LeftToRight
        train.Place(headPos, TravelDirection.LeftToRight);

        // Assert: The tail should be 30m into segB (50m on segA + 30m on segB = 80m length)
        Assert.NotNull(train.Tail);
        Assert.Equal(segB.Id, train.Tail.Segment.Id);
        Assert.Equal(70.0, train.Tail.DistanceFromLeftEnd, 1); // 100.0 (length) - 30.0 = 70.0
        Assert.Equal(train.Length, train.Path.Length, 1);
    }

    [Fact]
    public void Place_TrainTooLongForTrack_ThrowsInvalidOperationException()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        _network.AddBorderNode("border-L").Connect(new(segA, SegmentEnd.Left));
        var headPos = new TrackPosition(segA, 50.0);
        var train = new Train("T-02", 60.0, _simulationState, _navigator); // Train is 60m, but only 50m of track behind it

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => train.Place(headPos, TravelDirection.LeftToRight));
        Assert.Contains("train is too long", ex.Message);
    }

    [Fact]
    public void Place_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 200);
        var headPos1 = new TrackPosition(segA, 150.0);
        var headPos2 = new TrackPosition(segA, 180.0);
        var train = new Train("T-01", 100.0, _simulationState, _navigator);
        train.Place(headPos1, TravelDirection.LeftToRight); // First placement

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => train.Place(headPos2, TravelDirection.LeftToRight));
        Assert.Contains("has already been placed", ex.Message);
    }

    #endregion

    #region Movement Tests

    [Fact]
    public void Move_ForwardOnSingleSegment_UpdatesHeadAndTailCorrectly()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 500);
        // --- START OF CHANGE ---
        var train = new TestableTrain("T-01", 100, _simulationState, _navigator);
        train.Place(new TrackPosition(segA, 200), TravelDirection.LeftToRight); // Tail @ 100, Head @ 200

        // Act
        bool success = train.MoveProxy(PathDirection.Forward, 50);
        // --- END OF CHANGE ---

        // Assert
        Assert.True(success);
        Assert.NotNull(train.Path);
        Assert.Equal(150, train.Tail.DistanceFromLeftEnd);
        Assert.Equal(250, train.Head.DistanceFromLeftEnd);
        Assert.Equal(100, train.Path.Length, 1);
    }

    [Fact]
    public void Move_ForwardAcrossNode_UpdatesHeadAndTailCorrectly()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 300);
        var segB = _network.AddTrackSegment("seg-B", 200);
        _network.AddStraightNode("node-AB").Connect(new(segA, SegmentEnd.Right), new(segB, SegmentEnd.Left));

        // --- START OF CHANGE ---
        var train = new TestableTrain("T-01", 100, _simulationState, _navigator);
        train.Place(new TrackPosition(segA, 280), TravelDirection.LeftToRight);

        // Act
        bool success = train.MoveProxy(PathDirection.Forward, 40);
        // --- END OF CHANGE ---

        // Assert
        Assert.True(success);
        Assert.NotNull(train.Path);
        Assert.Equal(segB.Id, train.Head.Segment.Id);
        Assert.Equal(20, train.Head.DistanceFromLeftEnd, 1);
        Assert.Equal(segA.Id, train.Tail.Segment.Id);
        Assert.Equal(220, train.Tail.DistanceFromLeftEnd, 1);
    }

    [Fact]
    public void Move_MovesOffTrack_ReturnsFalseAndPathBecomesNull()
    {
        // Arrange
        var segA = _network.AddTrackSegment("seg-A", 100);
        _network.AddBorderNode("border-R").Connect(new(segA, SegmentEnd.Right));
        // --- START OF CHANGE ---
        var train = new TestableTrain("T-01", 50, _simulationState, _navigator);
        train.Place(new TrackPosition(segA, 80), TravelDirection.LeftToRight);

        // Act
        bool success = train.MoveProxy(PathDirection.Forward, 30);
        // --- END OF CHANGE ---

        // Assert
        Assert.False(success);
        Assert.Null(train.Path);
        Assert.Null(train.Head);
        Assert.Null(train.Tail);
    }

    [Fact]
    public void Move_ForwardAndBackwardAcrossSwitchback_ReturnsToOriginalPosition()
    {
        // ARRANGE

        // 1. Build a "Z" shaped track network with two direction reversals.
        //    A (L->R) --node1-- (R->L) B --node2-- (L->R) C
        var segA = _network.AddTrackSegment("seg-Z-A", 100);
        var segB = _network.AddTrackSegment("seg-Z-B", 100);
        var segC = _network.AddTrackSegment("seg-Z-C", 100);

        // Connection 1: A.Right -> B.Right (First reversal)
        var node1 = _network.AddStraightNode("node-Z-1");
        node1.Connect(new(segA, SegmentEnd.Right), new(segB, SegmentEnd.Right));

        // Connection 2: B.Left -> C.Left (Second reversal)
        var node2 = _network.AddStraightNode("node-Z-2");
        node2.Connect(new(segB, SegmentEnd.Left), new(segC, SegmentEnd.Left));

        // 2. Place a train so its body spans the first reversal (from segA to segB).
        var train = new TestableTrain("T-Switchback", 80, _simulationState, _navigator);
        // The natural path from A to B is L->R on A, then R->L on B.
        // We will place the head at 70m from the left end of B.
        var initialHeadPos = new TrackPosition(segB, 70.0);

        // Place the train facing "backward" relative to segment B's orientation,
        // which is "forward" along the path from A -> B -> C.
        train.Place(initialHeadPos, TravelDirection.RightToLeft);

        // 3. Verify initial placement and store original positions.
        // Head is at B, 70. It occupies 30m of B (from 100 to 70).
        // Tail is 50m into A (from 100 to 50).
        var originalHead = train.Head;
        var originalTail = train.Tail;

        Assert.NotNull(originalHead);
        Assert.NotNull(originalTail);
        Assert.Equal(segB.Id, originalHead.Segment.Id);
        Assert.Equal(70.0, originalHead.DistanceFromLeftEnd);
        Assert.Equal(segA.Id, originalTail.Segment.Id);
        Assert.Equal(50.0, originalTail.DistanceFromLeftEnd);

        double moveDistance = 40.0;

        // ACT 1: Move the train forward.
        bool forwardSuccess = train.MoveProxy(PathDirection.Forward, moveDistance);

        // ASSERT 1: Verify the new position.
        Assert.True(forwardSuccess);
        Assert.NotNull(train.Head);
        Assert.NotNull(train.Tail);

        // Head was at B, 70. Moved forward (R->L) by 40m. New pos: 70 - 40 = 30.
        Assert.Equal(segB.Id, train.Head.Segment.Id);
        Assert.Equal(30.0, train.Head.DistanceFromLeftEnd, 1);

        // Tail was at A, 50. Moved forward (L->R) by 40m. New pos: 50 + 40 = 90.
        Assert.Equal(segA.Id, train.Tail.Segment.Id);
        Assert.Equal(90.0, train.Tail.DistanceFromLeftEnd, 1);

        // ACT 2: Move the train backward by the same distance.
        bool backwardSuccess = train.MoveProxy(PathDirection.Backward, moveDistance);

        // ASSERT 2: Verify the train has returned to its exact original position.
        Assert.True(backwardSuccess);

        // Assert.Equal uses the record's value-based equality, which is perfect here.
        Assert.Equal(originalHead, train.Head);
        Assert.Equal(originalTail, train.Tail);
    }

    [Fact]
    public void Move_BeforePlacement_ThrowsInvalidOperationException()
    {
        // Arrange
        // --- START OF CHANGE ---
        var train = new TestableTrain("T-01", 100, _simulationState, _navigator);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => train.MoveProxy(PathDirection.Forward, 10));
        // --- END OF CHANGE ---
        Assert.Contains("has not been placed", ex.Message);
    }

    #endregion
}