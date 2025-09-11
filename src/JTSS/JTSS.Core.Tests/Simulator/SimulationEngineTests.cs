using JTSS.Core.Simulator;
using JTSS.Core.Simulator.Interfaces;

namespace JTSS.Core.Tests.Simulator;

// Test double is unchanged
internal class TestSimulatedElement : ISimulatedElement
{
    public int UpdateCount { get; private set; }
    public TimeSpan LastDeltaTime { get; private set; }

    public void Update(TimeSpan deltaTime)
    {
        UpdateCount++;
        LastDeltaTime = deltaTime;
    }
}

public class SimulationEngineTests
{
    [Fact]
    public void SimulationEngine_Constructor_WithInvalidTimeStep_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SimulationEngine(TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SimulationEngine(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void SimulationEngine_Step_AdvancesClockCorrectly()
    {
        // Arrange
        var timeStep = TimeSpan.FromMilliseconds(100);
        var simulationEngine = new SimulationEngine(timeStep);

        // Act
        simulationEngine.Step();
        simulationEngine.Step();

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(200), simulationEngine.State.CurrentTime);
    }

    [Fact]
    public void SimulationEngine_Step_CallsUpdateOnRegisteredElements()
    {
        // Arrange
        var timeStep = TimeSpan.FromMilliseconds(50);
        var simulationEngine = new SimulationEngine(timeStep);
        var element1 = new TestSimulatedElement();
        var element2 = new TestSimulatedElement();
        simulationEngine.RegisterElement(element1);
        simulationEngine.RegisterElement(element2);

        // Act
        simulationEngine.Step();

        // Assert
        Assert.Equal(1, element1.UpdateCount);
        Assert.Equal(timeStep, element1.LastDeltaTime);
        Assert.Equal(1, element2.UpdateCount);
        Assert.Equal(timeStep, element2.LastDeltaTime);
    }
}