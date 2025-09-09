using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Trains;
using System;
using Xunit;

namespace JTSS.Core.Tests;

// A simple test double for ISimulationState
internal class TestSimulationState : ISimulationState
{
    public TimeSpan CurrentTime { get; set; }
    public Simulator.Enums.SimulationStatus Status { get; set; }
}

public class TrainTests
{
    private readonly ISimulationState _simulationState;

    public TrainTests()
    {
        _simulationState = new TestSimulationState();
    }

    [Fact]
    public void Constructor_WithValidArgs_InitializesProperties()
    {
        // Arrange
        var train = new Train("T-01", 150.5, _simulationState, "Freight 1");

        // Assert
        Assert.Equal("T-01", train.Id);
        Assert.Equal("Freight 1", train.Name);
        Assert.Equal(150.5, train.Length);
        Assert.Null(train.Path); // Path should be null initially
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithNullId_ThrowsArgumentNullException(string invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Train(invalidId, 150.0, _simulationState));
    }

    [Fact]
    public void Constructor_WithNullSimulationState_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("simulationState", () => new Train("T-01", 150.0, null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidLength_ThrowsArgumentOutOfRangeException(double invalidLength)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>("length", () => new Train("T-01", invalidLength, _simulationState));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithEmptyOrWhitespaceId_ThrowsArgumentException(string invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Train(invalidId, 150.0, _simulationState));
    }

    [Fact]
    public void Update_DoesNotThrow()
    {
        // Arrange
        var train = new Train("T-02", 100.0, _simulationState);
        var timeSpan = TimeSpan.FromSeconds(1);

        // Act
        var exception = Record.Exception(() => train.Update(timeSpan));

        // Assert
        Assert.Null(exception);
    }
}