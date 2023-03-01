using System.Diagnostics;
using System.Diagnostics.Metrics;

#pragma warning disable CS8632

namespace AJProds.EFDataSeeder.Core;

/// <summary>
/// The default root instances for traces and metrics
/// </summary>
public static class Telemetries
{
    /// <summary>
    /// A prefix for all the projects
    /// </summary>
    private const string GlobalPrefix = "AJProds.EFDataSeeder";
        
    /// <summary>
    /// The default or root <see cref="ActivitySource"/>
    /// </summary>
    public static ActivitySource? DefaultActivitySource { get; } = new($"{GlobalPrefix}");

    /// <summary>
    /// The default or root <see cref="Meter"/>
    /// </summary>
    public static Meter? DefaultMeter { get; } = new ($"{GlobalPrefix}");
}