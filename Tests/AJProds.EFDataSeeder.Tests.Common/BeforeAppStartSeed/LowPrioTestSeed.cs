using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Tests.Common.BeforeAppStartSeed
{
    public class LowPrioTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 100;

        public string SeedName => "Low Prio seed";

        public SeedMode Mode => SeedMode.BeforeAppStart;

        public bool AlwaysRun => false;

        public LowPrioTestSeed(TestDbContext dbContext)
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