namespace JTSS.Core.Interfaces;

/// <summary>
/// Defines a generic, uniquely identifiable element within the simulation.
/// This is the root interface for all identifiable objects, both static and dynamic.
/// </summary>
public interface IIdentifiableElement
{
    /// <summary>
    /// The unique, user-defined identifier for this element.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// An optional, user-friendly name for this element. Does not need to be unique.
    /// </summary>
    string? Name { get; set; }
}
