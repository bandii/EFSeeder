﻿using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;

namespace AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;

public class NoneTestSeed: ISeed
{
    private readonly ITestContext _dbContext;

    public int Priority => 50;

    public string SeedName => "None seed";

    public SeedMode Mode => SeedMode.None;

    public bool AlwaysRun => false;

    public NoneTestSeed(ITestContext dbContext)
    {
        _dbContext = dbContext;
    }
        
    public async Task SeedAsync()
    {
        _dbContext.Testees.Add(new Testee
                               {
                                   Description = "None seed"
                               });

        await _dbContext.SaveChangesAsync();
    }
}