using JTSS.Core.Interfaces;
using JTSS.Core.Simulator;
using JTSS.Core.Simulator.Interfaces;
using JTSS.Core.Track;

namespace JTSS.Core.Tests;

// Test double is unchanged
internal class TestOperationalElement : IIdentifiableElement, ISimulatedElement
{
    public string Id { get; }
    public string? Name { get; set; }
    public int UpdateCount { get; private set; }

    public TestOperationalElement(string id, string? name = null)
    {
        Id = id;
        Name = name;
    }

    public void Update(TimeSpan deltaTime)
    {
        UpdateCount++;
    }
}

public class WorldTests
{
    // --- START OF CHANGE ---
    private readonly TrackNetwork _network;
    private readonly SimulationEngine _simulationEngine; // <-- No more CS0118 error
    private readonly World _world;
    // --- END OF CHANGE ---

    public WorldTests()
    {
        _network = new TrackNetwork();
        // --- START OF CHANGE ---
        _simulationEngine = new SimulationEngine(TimeSpan.FromSeconds(1));
        _world = new World(_network, _simulationEngine);
        // --- END OF CHANGE ---
    }

    [Fact]
    public void Constructor_WithValidArgs_InitializesProperties()
    {
        // Assert
        Assert.Equal(_network, _world.Network);
        Assert.Equal(_simulationEngine, _world.SimulationEngine); // <-- Property name changed
    }

    [Fact]
    public void Constructor_WithNullNetwork_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("network", () => new World(null!, _simulationEngine));
    }

    [Fact]
    public void Constructor_WithNullSimulator_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>("simulationEngine", () => new World(_network, null!)); // <-- Parameter name changed
    }

    [Fact]
    public void RegisterOperationalElement_AddsElementToCollection()
    {
        // Arrange
        var element = new TestOperationalElement("op-1", "Test Element");

        // Act
        _world.RegisterOperationalElement(element);
        var retrieved = _world.GetOperationalElementById("op-1");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(element, retrieved);
        Assert.Equal("Test Element", retrieved.Name);
    }

    [Fact]
    public void RegisterOperationalElement_WhenElementIsSimulated_AlsoRegistersWithSimulator()
    {
        // Arrange
        var element = new TestOperationalElement("op-sim-1");

        // Act
        _world.RegisterOperationalElement(element);
        _world.SimulationEngine.Step(); // <-- Property name changed

        // Assert
        Assert.Equal(1, element.UpdateCount);
    }

    [Fact]
    public void RegisterOperationalElement_WithDuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var element1 = new TestOperationalElement("op-duplicate");
        var element2 = new TestOperationalElement("op-duplicate");
        _world.RegisterOperationalElement(element1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _world.RegisterOperationalElement(element2));
    }



    [Fact]
    public void GetOperationalElementById_ForNonExistentId_ReturnsNull()
    {
        // Act
        var result = _world.GetOperationalElementById("non-existent");

        // Assert
        Assert.Null(result);
    }
}