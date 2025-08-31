namespace JTSS.Core.Track;

/// <summary>
/// Provides static, simulation-wide values for floating-point precision and tolerance.
/// </summary>
public static class TrackPrecision
{
    /// <summary>
    /// The tolerance for comparing floating-point distances. Positions are considered
    /// equal if the distance between them is less than or equal to this value.
    /// A value of 0.1m (10cm) is a reasonable starting point.
    /// </summary>
    public const double Tolerance = 0.1;
}
