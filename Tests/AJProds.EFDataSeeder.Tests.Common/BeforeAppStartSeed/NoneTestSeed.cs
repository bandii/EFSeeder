using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.BeforeAppStartSeed
{
    public class NoneTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 50;

        public string SeedName => "None seed";

        public SeedMode Mode => SeedMode.None;

        public bool AlwaysRun => false;

        public NoneTestSeed(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "None seed"
                                   });
            
            Console.WriteLine("!!! No record should be added by " + nameof(NoneTestSeed) + " - " + DateTime.Now);

            await _dbContext.SaveChangesAsync();
        }
    }
}