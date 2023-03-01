using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJProds.EFDataSeeder.Tests.Console;

/// <summary>
/// A common console app to demonstrate the tool
/// </summary>
class Program
{
    // Use the docker-compose.yml
    public const string ConnectionPostgresTest = @"Host=localhost;Database=Seeder;Username=admin;Password=password123";

    static async Task Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder()
                             .ConfigureServices(ConfigureServicesForPostgreSql())
                             .Build();

        await host.MigrateThenRunAsync(provider =>
                                           // Ensure the TestPostgreSQLDbContext's migration has run on start
                                           provider.GetRequiredService<ITestContext>()
                                                   .Database.MigrateAsync());
    }

    private static Action<IServiceCollection> ConfigureServicesForPostgreSql()
    {
        return collection =>
               {
                   RegisterSeeders(collection);

                   // Custom, test setup
                   // 1. StartUp project is this console app
                   // 2. The migration needs to be in the Tests.Common\Migrations folder
                   // 3. So run this script:
                   // dotnet ef migrations add --project ..\Tests\AJProds.EFDataSeeder.Tests.Common\AJProds.EFDataSeeder.Tests.Common.csproj --startup-project ..\Tests\AJProds.EFDataSeeder.Tests.Postgres.Console\AJProds.EFDataSeeder.Tests.Postgres.Console.csproj --context AJProds.EFDataSeeder.Tests.Common.TestPostgreSQLDbContext --configuration Debug TestMigration --output-dir Migrations\PostgreSQL
                   collection.AddDbContext<ITestContext, TestPostgreSQLDbContext>(builder => builder
                                                                                     .UseNpgsql(ConnectionPostgresTest));

                   // EFSeeder setup - with an own EF Migration History table
                   PostgreSql.Extensions.RegisterDataSeederServices(collection, ConnectionPostgresTest);
               };
    }

    private static void RegisterSeeders(IServiceCollection collection)
    {
        collection.RegisterDataSeeder<HighPrioTestSeed>();
        collection.RegisterDataSeeder<LowPrioTestSeed>();
        collection.RegisterDataSeeder<AlwaysRunTestSeed>();
        collection.RegisterDataSeeder<NoneTestSeed>();
    }
}