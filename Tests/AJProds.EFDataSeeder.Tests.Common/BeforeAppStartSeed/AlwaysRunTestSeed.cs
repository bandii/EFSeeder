using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Tests.Common.BeforeAppStartSeed
{
    public class AlwaysRunTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 50;

        public string SeedName => "Always Run seed";

        public SeedMode Mode => SeedMode.BeforeAppStart;

        public bool AlwaysRun => true;

        public AlwaysRunTestSeed(TestDbContext dbContext)
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
}