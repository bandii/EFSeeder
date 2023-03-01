namespace AJProds.EFDataSeeder.Core;

/// <summary>
/// Determines, when to run the <see cref="ISeed"/> implementations
/// </summary>
public enum SeedMode
{
    /// <summary>
    /// Not defined.
    /// Consumer will determine when and how to use <see cref="BaseSeederManager"/>
    /// </summary>
    None,

    /// <summary>
    /// Seed before application start event, after migrations
    /// </summary>
    BeforeAppStart,

    /// <summary>
    /// Seed after application start event.
    /// </summary>
    AfterAppStart
}