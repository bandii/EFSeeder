using System.Threading.Tasks;

namespace AJProds.EFDataSeeder.Tests.Console
{
    public class NewerTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 100;

        public string SeedName => "Low Prio seed";

        public SeedMode Mode => SeedMode.BeforeAppStart;

        public bool AlwaysRun => false;

        public NewerTestSeed(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "Something Newer (2)"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}