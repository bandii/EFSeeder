﻿using System;
using System.Threading;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJProds.EFDataSeeder.Core;

public static class Extensions
{
    /// <summary>
    /// Registers all the necessary tools to be able to use EFDataSeeder
    /// </summary>
    /// <param name="collection"><see cref="IServiceCollection"/></param>
    /// <param name="dbConnectionOptions"><see cref="DbContextOptionsBuilder"/></param>
    /// <param name="schemaName">The name of the schema, what the nuget will use</param>
    public static IServiceCollection RegisterDataSeederServices(this IServiceCollection collection,
                                                                Action<DbContextOptionsBuilder> dbConnectionOptions,
                                                                string schemaName = "sdr")
    {
        // Register tools
        collection.AddTransient<BaseSeederManager>();
        collection.AddHostedService<SeederHostedService>();

        SeederDbContext.SCHEMA = schemaName;
        collection.AddDbContext<SeederDbContext>(dbConnectionOptions);

        return collection;
    }

    /// <summary>
    /// Register your data seeders that need to be run
    /// </summary>
    /// <typeparam name="TSeeder">The <see cref="ISeed"/> implementation</typeparam>
    public static IServiceCollection RegisterDataSeeder<TSeeder>(this IServiceCollection collection)
        where TSeeder : class, ISeed
    {
        collection.TryAddEnumerable(ServiceDescriptor.Transient<ISeed, TSeeder>());

        return collection;
    }

    /// <summary>
    /// Runs the seed procedures, migrations before app start
    /// </summary>
    /// <param name="host"><see cref="IHostBuilder"/></param>
    /// <param name="actionBeforeRun">Apply your custom logic to be run before seed procedures run.
    /// For example: run your db context migrations here.
    /// </param>
    /// <param name="cts"><see cref="CancellationToken"/></param>
    public static async Task MigrateThenRunAsync(this IHost host,
                                                 Func<IServiceProvider, Task> actionBeforeRun = null,
                                                 CancellationToken cts = default)
    {
        using (var scope = host.Services
                               .GetRequiredService<IServiceScopeFactory>()
                               .CreateScope())
        {
            var dbContext = scope.ServiceProvider
                                 .GetRequiredService<SeederDbContext>();

            try
            {
                // Custom migration, logic to be run
                if (actionBeforeRun != null)
                {
                    await actionBeforeRun(scope.ServiceProvider)
                       .ConfigureAwait(false);
                }

                // Migrate own schema changes
                if (dbContext.Database.IsRelational())
                {
                    await dbContext.Database.MigrateAsync(cts);
                }
                else // We are using for example an inMemory db
                {
                    await dbContext.Database.EnsureCreatedAsync(cts);
                }

                // Seed before app start, after migration
                await scope.ServiceProvider
                           .GetRequiredService<BaseSeederManager>()
                           .SeedAsync(SeedMode.BeforeAppStart, cts)
                           .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var logger = scope.ServiceProvider.GetService<ILogger>();
                logger?.LogError(e, "Migration could not run");

                throw;
            }
        }

        await host.StartAsync(cts)
                  .ConfigureAwait(false);
    }
}