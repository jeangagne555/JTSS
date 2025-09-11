using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Trains;

namespace JTSS.Core.Tests.Classes;

/// <summary>
/// A testing-specific subclass of Train that exposes the protected Move method for unit testing.
/// </summary>
internal class TestableTrain : Train
{
    // The constructor must call the base constructor to pass all the dependencies.
    public TestableTrain(string id, double length, ISimulationState simulationState, ITrackNavigator navigator, string? name = null)
        : base(id, length, simulationState, navigator, name)
    {
    }

    /// <summary>
    /// A public proxy method that calls the protected base Move method.
    /// </summary>
    public bool MoveProxy(PathDirection direction, double distanceInMeters)
    {
        return this.Move(direction, distanceInMeters);
    }
}