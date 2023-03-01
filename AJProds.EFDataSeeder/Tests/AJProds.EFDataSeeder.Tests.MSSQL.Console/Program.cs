using System;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;
using AJProds.EFDataSeeder.Tests.Console;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// A common console app to demonstrate the tool

using var host = Host.CreateDefaultBuilder()
                     .ConfigureServices(ConfigureServicesForMsSql())
                     .Build();

await host.MigrateThenRunAsync(provider =>
                                   // Ensure the TestMSSQLDbContext's migration has run on start
                                   provider.GetRequiredService<ITestContext>()
                                           .Database.MigrateAsync());

static Action<IServiceCollection> ConfigureServicesForMsSql()
{
    return collection =>
           {
               RegisterSeeders(collection);

               // Custom, test setup
               // 1. StartUp project is this console app
               // 2. The migration needs to be in the Tests.Common\Migrations folder
               // 3. So run this script:
               // dotnet ef migrations add --project ..\Tests\AJProds.EFDataSeeder.Tests.Common\AJProds.EFDataSeeder.Tests.Common.csproj --startup-project ..\Tests\AJProds.EFDataSeeder.Tests.MSSQL.Console\AJProds.EFDataSeeder.Tests.MSSQL.Console.csproj --context AJProds.EFDataSeeder.Tests.Common.TestMSSQLDbContext --configuration Debug TestMigration --output-dir Migrations\MSSQL
               collection.AddDbContext<ITestContext, TestMSSQLDbContext>(builder => builder
                                                                            .UseSqlServer(Constants.ConnectionMssqlTest
                                                                                          // It is not necessary, just an example
                                                                                          // , optionsBuilder =>
                                                                                          //      optionsBuilder
                                                                                          //         .MigrationsHistoryTable("__EFMigrationsHistory",
                                                                                          //              TestMSSQLDbContext.SCHEMA)
                                                                                         ));

               // EFSeeder setup - with an own EF Migration History table
               AJProds.EFDataSeeder.MsSql.Extensions.RegisterDataSeederServices(collection, Constants.ConnectionMssqlTest);
           };
}

static void RegisterSeeders(IServiceCollection collection)
{
    collection.RegisterDataSeeder<HighPrioTestSeed>();
    collection.RegisterDataSeeder<LowPrioTestSeed>();
    collection.RegisterDataSeeder<AlwaysRunTestSeed>();
    collection.RegisterDataSeeder<NoneTestSeed>();
}
