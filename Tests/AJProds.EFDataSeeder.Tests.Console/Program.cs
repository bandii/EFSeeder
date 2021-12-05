using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJProds.EFDataSeeder.Tests.Console
{
    /// <summary>
    /// A common console app to demonstrate the tool
    /// </summary>
    class Program
    {
        public const string CONNECTION_LOCAL_TEST =
            @"Server=localhost\SQLEXPRESS;Initial Catalog=SeederTest;Trusted_Connection=True;MultipleActiveResultSets=true";

        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                      .ConfigureServices(ConfigureServices())
                      .Build()
                      .MigrateThenRunAsync(provider =>
                                               // Ensure the TestDbContext's migration has run on start
                                               provider.GetRequiredService<TestDbContext>()
                                                       .Database.MigrateAsync());
        }

        private static Action<IServiceCollection> ConfigureServices()
        {
            return collection =>
                   {
                       // Register seeders
                       collection.RegisterDataSeeder<HighPrioTestSeed>();
                       collection.RegisterDataSeeder<LowPrioTestSeed>();
                       collection.RegisterDataSeeder<AlwaysRunTestSeed>();
                       collection.RegisterDataSeeder<NoneTestSeed>();

                       // My own, custom, test setup
                       // 1. StartUp project is this console app, and the migration needs to be here
                       // 2. So run this script from its root without the --project param
                       // dotnet ef migrations add InitialCreate --project ..\AJProds.EFDataSeeder.Tests.Common\ --context TestDbContext
                       collection.AddDbContext<TestDbContext>(builder => builder
                                                                 .UseSqlServer(CONNECTION_LOCAL_TEST
                                                                               // It is not necessary, just an example
                                                                               // , optionsBuilder =>
                                                                               //      optionsBuilder
                                                                               //         .MigrationsHistoryTable("__EFMigrationsHistory",
                                                                               //              TestDbContext.SCHEMA)
                                                                              ));

                       // EFSeeder setup - with an own EF Migration History table
                       // 1. StartUp project is this console app, and the migration needs to be here
                       // 2. So run this script from its root without the --project param
                       // dotnet ef migrations add InitialCreate --project ..\..\AJProds.EFDataSeeder\AJProds.EFDataSeeder\ --context SeederDbContext
                       collection.RegisterDataSeederServices(builder =>
                                                             {
                                                                 builder
                                                                    .UseSqlServer(CONNECTION_LOCAL_TEST
                                                                                  // I use it to generate the migration scripts
                                                                                  // , optionsBuilder =>
                                                                                  //      optionsBuilder
                                                                                  //         .MigrationsHistoryTable("__EFSeederMigrationsHistory",
                                                                                  //              SeederDbContext.SCHEMA)
                                                                                 );
                                                             });
                   };
        }
    }
}