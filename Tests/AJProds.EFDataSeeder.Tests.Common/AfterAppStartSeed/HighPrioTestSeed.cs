using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed
{
    public class HighPrioTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 10;

        public string SeedName => "High Prio seed";

        public SeedMode Mode => SeedMode.AfterAppStart;

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