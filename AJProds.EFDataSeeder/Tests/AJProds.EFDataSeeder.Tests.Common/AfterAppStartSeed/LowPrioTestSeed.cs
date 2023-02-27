using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed
{
    public class LowPrioTestSeed: ISeed
    {
        private readonly ITestContext _dbContext;

        public int Priority => 100;

        public string SeedName => "Low Prio seed";

        public SeedMode Mode => SeedMode.AfterAppStart;

        public bool AlwaysRun => false;

        public LowPrioTestSeed(ITestContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "Low Prio seed"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}