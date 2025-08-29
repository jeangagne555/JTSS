using JTSS.Core;
using JTSS.Core.Enums;
using JTSS.Core.Interfaces;
using Moq;
using Xunit;

namespace JTSS.Core.Tests
{
    public class SwitchNodeTests
    {
        [Fact]
        public void CanTravel_NormalState_FacingToTrailing_ReturnsTrue()
        {
            var segmentA = new Mock<ITrackSegment>().Object;
            var segmentB = new Mock<ITrackSegment>().Object;
            var segmentC = new Mock<ITrackSegment>().Object;

            var node = new SwitchNode(
                (segmentA, SegmentEnd.Start),
                (segmentB, SegmentEnd.End),
                (segmentC, SegmentEnd.Start),
                SwitchState.Normal);

            Assert.True(node.CanTravel((segmentA, SegmentEnd.Start), (segmentB, SegmentEnd.End)));
            Assert.True(node.CanTravel((segmentB, SegmentEnd.End), (segmentA, SegmentEnd.Start)));
            Assert.False(node.CanTravel((segmentA, SegmentEnd.Start), (segmentC, SegmentEnd.Start)));
        }

        [Fact]
        public void CanTravel_ReversedState_FacingToDiverging_ReturnsTrue()
        {
            var segmentA = new Mock<ITrackSegment>().Object;
            var segmentB = new Mock<ITrackSegment>().Object;
            var segmentC = new Mock<ITrackSegment>().Object;

            var node = new SwitchNode(
                (segmentA, SegmentEnd.Start),
                (segmentB, SegmentEnd.End),
                (segmentC, SegmentEnd.Start),
                SwitchState.Reversed);

            Assert.True(node.CanTravel((segmentA, SegmentEnd.Start), (segmentC, SegmentEnd.Start)));
            Assert.True(node.CanTravel((segmentC, SegmentEnd.Start), (segmentA, SegmentEnd.Start)));
            Assert.False(node.CanTravel((segmentA, SegmentEnd.Start), (segmentB, SegmentEnd.End)));
        }
    }
}