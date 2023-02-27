using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed
{
    public class LengthyHighTestSeed: ISeed
    {
        private readonly ITestContext _dbContext;

        public int Priority => 0;

        public string SeedName => "None seed";

        public SeedMode Mode => SeedMode.AfterAppStart;

        public bool AlwaysRun => false;

        public LengthyHighTestSeed(ITestContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            await Task.Delay(TimeSpan.FromMinutes(5)); // The lengthy operation
            
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "Lengthy seed"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}