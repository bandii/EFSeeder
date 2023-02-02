﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core.Db;

using Microsoft.Extensions.Logging;

namespace AJProds.EFDataSeeder.Core
{
    public class BaseSeederManager
    {
        private readonly IEnumerable<ISeed> _seeders;

        private readonly SeederDbContext _dbContext;

        private readonly ILogger<BaseSeederManager> _logger;

        public BaseSeederManager(IEnumerable<ISeed> seeders,
                                 SeederDbContext dbContext,
                                 ILogger<BaseSeederManager> logger)
        {
            _seeders = seeders;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Runs the <see cref="ISeed"/> implementations according to the <paramref name="when"/>
        /// </summary>
        /// <param name="when">When is this function get called?</param>
        public virtual async Task SeedAsync(SeedMode when = SeedMode.None)
        {
            using var startActivity = Telemetries.DefaultActivitySource?.StartActivity("Starting up seeders");
            
            startActivity?.AddEvent(new ActivityEvent("Seeding started for SeedMode: " + Enum.GetName(when)));
            _logger.LogInformation("Seeding started");

            var seedAlreadyRun = _dbContext.SeederHistories
                                           .ToList();

            var seeds = _seeders
                       .Where(seed => seed.Mode == when
                                   && (seed.AlwaysRun
                                    || seedAlreadyRun.All(history => history.SeedName != seed.SeedName)))
                       .OrderBy(seed => seed.Priority)
                       .ToList();

            _logger.LogInformation("Seeding started with {SeedsCount} to be run", seeds.Count);
            foreach (var seeder in seeds)
            {
                startActivity?.AddEvent(new ActivityEvent($"Seeding {seeder.SeedName}.."));
                _logger.LogInformation("Seeding {SeederSeedName}..", seeder.SeedName);
                
                await seeder.SeedAsync();
                
                await SaveHistoryLog(seeder, seedAlreadyRun);
            }
        }

        private async Task SaveHistoryLog(ISeed seeder, IReadOnlyCollection<SeederHistory> seedAlreadyRun)
        {
            // One-time run == insert
            if (!seeder.AlwaysRun
             || (seeder.AlwaysRun && seedAlreadyRun.All(history => history.SeedName != seeder.SeedName)))
            {
                _dbContext.SeederHistories.Add(new SeederHistory
                                               {
                                                   SeedName = seeder.SeedName,
                                                   AlwaysRun = seeder.AlwaysRun,
                                                   FirstRunAt = DateTime.Now,
                                                   LastRunAt = DateTime.Now
                                               });
            }
            else
            {
                var seederHistory = seedAlreadyRun
                   .Single(history => history.SeedName == seeder.SeedName);

                seederHistory.SeedName = seeder.SeedName;
                seederHistory.AlwaysRun = seeder.AlwaysRun;
                seederHistory.LastRunAt = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}