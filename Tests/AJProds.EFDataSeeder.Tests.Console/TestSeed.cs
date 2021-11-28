using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Tests.Console
{
    public class TestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 0;

        public string SeedName => "High Prio seed";

        public SeedMode Mode => SeedMode.AfterAppStart;

        public bool AlwaysRun => false;

        public TestSeed(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "Something 1"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}