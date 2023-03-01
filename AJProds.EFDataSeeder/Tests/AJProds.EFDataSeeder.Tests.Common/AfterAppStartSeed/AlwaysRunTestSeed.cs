using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;

public class AlwaysRunTestSeed: ISeed
{
    private readonly ITestContext _dbContext;

    public int Priority => 50;

    public string SeedName => "Always Run seed";

    public SeedMode Mode => SeedMode.AfterAppStart;

    public bool AlwaysRun => true;

    public AlwaysRunTestSeed(ITestContext dbContext)
    {
        _dbContext = dbContext;
    }
        
    public async Task SeedAsync()
    {
        _dbContext.Testees.Add(new Testee
                               {
                                   Description = "Always Run seed"
                               });

        await _dbContext.SaveChangesAsync();
    }
}