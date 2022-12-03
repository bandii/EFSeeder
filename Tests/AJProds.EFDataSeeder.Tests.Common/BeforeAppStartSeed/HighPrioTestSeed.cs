using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.BeforeAppStartSeed
{
    public class HighPrioTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 0;

        public string SeedName => "High Prio seed";

        public SeedMode Mode => SeedMode.BeforeAppStart;

        public bool AlwaysRun => false;

        public HighPrioTestSeed(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "High Prio seed"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}