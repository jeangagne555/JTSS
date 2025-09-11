using JTSS.Core.Track;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Tests.Track;

public class SwitchNodeTests
{
    // A helper enum for readability in the test data
    public enum SegmentRole { Facing, Trailing, Diverging }

    [Theory]
    [MemberData(nameof(PathingData))]
    public void GetValidPaths_ReturnsCorrectPath_BasedOnStateAndEntry(
        SegmentRole entryRole, SwitchState switchState, SegmentRole? expectedExitRole)
    {
        // Arrange
        var sw = new SwitchNode("sw-1");
        var facingSeg = new TrackSegment("seg-facing", 100);
        var trailingSeg = new TrackSegment("seg-trailing", 100);
        var divergingSeg = new TrackSegment("seg-diverging", 100);

        sw.Connect(
            new TrackConnection(facingSeg, SegmentEnd.Right),
            new TrackConnection(trailingSeg, SegmentEnd.Left),
            new TrackConnection(divergingSeg, SegmentEnd.Left)
        );

        sw.State = switchState;

        var segments = new Dictionary<SegmentRole, ITrackSegment>
        {
            { SegmentRole.Facing, facingSeg },
            { SegmentRole.Trailing, trailingSeg },
            { SegmentRole.Diverging, divergingSeg }
        };

        var entrySegment = segments[entryRole];

        // Act
        var paths = sw.GetValidPaths(entrySegment).ToList();

        // Assert
        if (expectedExitRole.HasValue)
        {
            var expectedSegment = segments[expectedExitRole.Value];
            var actualSegment = Assert.Single(paths);
            Assert.Equal(expectedSegment, actualSegment);
        }
        else
        {
            Assert.Empty(paths);
        }
    }

    public static IEnumerable<object[]> PathingData =>
        new List<object[]>
        {
            // Valid paths
            new object[] { SegmentRole.Facing, SwitchState.Normal, SegmentRole.Trailing },
            new object[] { SegmentRole.Facing, SwitchState.Reversed, SegmentRole.Diverging },
            new object[] { SegmentRole.Trailing, SwitchState.Normal, SegmentRole.Facing },
            new object[] { SegmentRole.Diverging, SwitchState.Reversed, SegmentRole.Facing },
            // Invalid paths (blocked by switch)
            new object[] { SegmentRole.Trailing, SwitchState.Reversed, null },
            new object[] { SegmentRole.Diverging, SwitchState.Normal, null },
        };
}
