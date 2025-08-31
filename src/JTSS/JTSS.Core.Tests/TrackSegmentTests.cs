using JTSS.Core.Track;
using JTSS.Core.Track.Enums;

namespace JTSS.Core.Tests;

public class TrackSegmentTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesPropertiesCorrectly()
    {
        // Arrange & Act
        var segment = new TrackSegment("seg-1", 150.5);

        // Assert
        Assert.Equal("seg-1", segment.Id);
        Assert.Equal(150.5, segment.Length);
        Assert.Null(segment.LeftEndNode);
        Assert.Null(segment.RightEndNode);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-100.0)]
    public void Constructor_WithInvalidLength_ThrowsArgumentOutOfRangeException(double invalidLength)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new TrackSegment("seg-1", invalidLength));
    }

    [Theory]
    [InlineData(null)]
    public void Constructor_WithInvalidId_ThrowsArgumentNullException(string invalidId)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TrackSegment(invalidId, 100.0));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidId_ThrowsArgumentException(string invalidId)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new TrackSegment(invalidId, 100.0));
    }

    [Fact]
    public void SetNodeAt_ConnectsNodesToCorrectEnds()
    {
        // Arrange
        var segment = new TrackSegment("seg-1", 100);
        var nodeLeft = new BorderNode("node-L");
        var nodeRight = new BorderNode("node-R");

        // Act
        segment.SetNodeAt(SegmentEnd.Left, nodeLeft);
        segment.SetNodeAt(SegmentEnd.Right, nodeRight);

        // Assert
        Assert.Equal(nodeLeft, segment.LeftEndNode);
        Assert.Equal(nodeRight, segment.RightEndNode);
    }

    [Fact]
    public void SetNodeAt_ConnectingToAlreadyConnectedEnd_ThrowsInvalidOperationException()
    {
        // Arrange
        var segment = new TrackSegment("seg-1", 100);
        var node1 = new BorderNode("node-1");
        var node2 = new BorderNode("node-2");
        segment.SetNodeAt(SegmentEnd.Left, node1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => segment.SetNodeAt(SegmentEnd.Left, node2));
    }

    [Fact]
    public void GetOppositeNode_GivenConnectedNode_ReturnsCorrectOppositeNode()
    {
        // Arrange
        var segment = new TrackSegment("seg-1", 100);
        var nodeLeft = new BorderNode("node-L");
        var nodeRight = new BorderNode("node-R");
        segment.SetNodeAt(SegmentEnd.Left, nodeLeft);
        segment.SetNodeAt(SegmentEnd.Right, nodeRight);

        // Act
        var resultFromLeft = segment.GetOppositeNode(nodeLeft);
        var resultFromRight = segment.GetOppositeNode(nodeRight);

        // Assert
        Assert.Equal(nodeRight, resultFromLeft);
        Assert.Equal(nodeLeft, resultFromRight);
    }

    [Fact]
    public void GetOppositeNode_GivenUnconnectedNode_ThrowsArgumentException()
    {
        // Arrange
        var segment = new TrackSegment("seg-1", 100);
        var nodeLeft = new BorderNode("node-L");
        var nodeRight = new BorderNode("node-R");
        var unrelatedNode = new BorderNode("unrelated");
        segment.SetNodeAt(SegmentEnd.Left, nodeLeft);
        segment.SetNodeAt(SegmentEnd.Right, nodeRight);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => segment.GetOppositeNode(unrelatedNode));
    }
}