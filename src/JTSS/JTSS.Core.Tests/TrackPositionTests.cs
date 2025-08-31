using JTSS.Core.Track;
using JTSS.Core.Track.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSS.Core.Tests;

public class TrackPositionTests
{
    private readonly ITrackSegment _segment;

    // The constructor runs before each test, providing a fresh segment instance.
    public TrackPositionTests()
    {
        _segment = new TrackSegment("seg-test", 100.0);
    }

    [Fact]
    public void Constructor_WithValidArguments_InitializesPropertiesCorrectly()
    {
        // Arrange
        double distance = 50.0;

        // Act
        var position = new TrackPosition(_segment, distance);

        // Assert
        Assert.Equal(_segment, position.Segment);
        Assert.Equal(distance, position.DistanceFromLeftEnd);
    }

    [Theory]
    [InlineData(0.0)]     // Boundary: At the very beginning
    [InlineData(100.0)]   // Boundary: At the very end
    public void Constructor_WithBoundaryDistances_Succeeds(double validDistance)
    {
        // Act
        var position = new TrackPosition(_segment, validDistance);

        // Assert
        // The fact that no exception was thrown is the assertion.
        Assert.Equal(validDistance, position.DistanceFromLeftEnd);
    }

    [Fact]
    public void Constructor_WithNullSegment_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>("segment", () => new TrackPosition(null!, 50.0));
    }

    [Theory]
    [InlineData(-0.001)]  // Just below the lower boundary
    [InlineData(100.001)] // Just above the upper boundary
    [InlineData(-50.0)]   // Well below the lower boundary
    public void Constructor_WithOutOfRangeDistance_ThrowsArgumentOutOfRangeException(double invalidDistance)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>("distanceFromLeftEnd", () => new TrackPosition(_segment, invalidDistance));
    }

    [Fact]
    public void Equality_WithIdenticalValues_ReturnsTrue()
    {
        // Arrange
        // Create two separate instances with the same values
        var pos1 = new TrackPosition(_segment, 75.5);
        var pos2 = new TrackPosition(_segment, 75.5);

        // Act & Assert
        // Assert.Equal uses the .Equals() method, which is overridden for records.
        Assert.Equal(pos1, pos2);
        Assert.True(pos1 == pos2);
        Assert.False(pos1 != pos2);
    }

    [Fact]
    public void Equality_WithDifferentDistances_ReturnsFalse()
    {
        // Arrange
        var pos1 = new TrackPosition(_segment, 75.5);
        var pos2 = new TrackPosition(_segment, 75.6);

        // Act & Assert
        Assert.NotEqual(pos1, pos2);
        Assert.False(pos1 == pos2);
        Assert.True(pos1 != pos2);
    }

    [Fact]
    public void Equality_WithDifferentSegments_ReturnsFalse()
    {
        // Arrange
        var anotherSegment = new TrackSegment("another-seg", 100.0);
        var pos1 = new TrackPosition(_segment, 50.0);
        var pos2 = new TrackPosition(anotherSegment, 50.0); // Same distance, different segment

        // Act & Assert
        Assert.NotEqual(pos1, pos2);
        Assert.False(pos1 == pos2);
        Assert.True(pos1 != pos2);
    }
}
