using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJProds.EFDataSeeder.Tests.Console
{
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
                                               // Ensure the TestDbContext's migration is run on start
                                               provider.GetRequiredService<TestDbContext>()
                                                       .Database.MigrateAsync());
        }

        private static Action<IServiceCollection> ConfigureServices()
        {
            return collection =>
                   {
                       // Register seeders
                       collection.RegisterDataSeeder<TestSeed>();
                       collection.RegisterDataSeeder<NewerTestSeed>();
                       
                       // My own, custom, test setup
                       // 1. StartUp project is this console app, and the migration needs to be here
                       // 2. So run this script from its root without the --project param
                       // dotnet ef migrations add InitialCreate --context TestDbContext
                       collection.AddDbContext<TestDbContext>(builder => builder
                                                                 .UseSqlServer(CONNECTION_LOCAL_TEST,
                                                                               optionsBuilder =>
                                                                                   optionsBuilder
                                                                                      .MigrationsHistoryTable("__EFMigrationsHistory",
                                                                                           TestDbContext.SCHEMA)));

                       // EFSeeder setup - with an own EF Migration History table
                       // 1. StartUp project is this console app, and the migration needs to be here
                       // 2. So run this script from its root without the --project param
                       // dotnet ef migrations add InitialCreate --project ..\..\AJProds.EFDataSeeder\AJProds.EFDataSeeder\ --context SeederDbContext
                       collection.RegisterDataSeederServices(builder =>
                                                             {
                                                                 builder
                                                                    .UseSqlServer(CONNECTION_LOCAL_TEST,
                                                                                  optionsBuilder =>
                                                                                      optionsBuilder
                                                                                         .MigrationsHistoryTable("__EFSeederMigrationsHistory",
                                                                                              SeederDbContext.SCHEMA));
                                                             });
                   };
        }
    }
}