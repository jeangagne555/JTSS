using JTSS.Core.Simulator.Enums;
using JTSS.Core.Simulator.Interfaces;
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
    // --- END OF NEW REGION ---
}