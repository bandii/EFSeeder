using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Core;

/// <summary>
/// Seed procedure implementation.
/// Register your seed implementations with the <see cref="Extensions.RegisterDataSeeder{TSeeder}"/>
/// </summary>
/// <remarks>
/// <see cref="Extensions.RegisterDataSeeder{TSeeder}"/> can register multiple implementation types
/// behind the <see cref="ISeed"/> interface
/// </remarks>
public interface ISeed
{
    /// <summary>
    /// The lower priority (number) will run earlier
    /// </summary>
    public int Priority { get; }
        
    /// <summary>
    /// Unique identifier for this seed.
    /// </summary>
    public string SeedName { get; }

    /// <inheritdoc cref="SeedMode"/>
    public SeedMode Mode { get; }
    
    /// <summary>
    /// Should this seed run everytime when the seed procedures got triggered?
    /// </summary>
    public bool AlwaysRun { get; }

    /// <summary>
    /// Runs the data injection
    /// </summary>
    public Task SeedAsync();
}