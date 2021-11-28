## Problem
There might be situations, when you need to seed some data with keeping the business
logic in mind.
Applying business logic and versioning of SQL migration scripts can be time-consuming.
We should not forget about it would be great to be able to trace back the changes done in 
the database.

## Solution
Let's seed this data with using the existing logic already composed in your solution. 
Let's do it with simply registering `ISeed` implementations to your ioc,
then let's check in the db, the `sdr.SeederHistories` table, whether your procedure has been run.

You can define when the `ISeed` implementations should be run. See the following options.

### Options - Mode => None
No ISeed logic will be run. You need to find the `BaseSeederManager` in your ioc 
and run it with the `SeedMode.None` argument.
```cs
var baseSeederManager = provider.GetRequiredService<BaseSeederManager>();
baseSeederManager.Seed(SeedMode.None);
```

### Options - Mode => BeforeAppStart
You must use the `Extensions.MigrateThenRunAsync` with your `HostBuilder`,
because this procedure will not start up the host until all migration
and seed (with _`SeedMode.BeforeAppStart`_) has been run successfully.

```cs
await Host.CreateDefaultBuilder()
          .ConfigureServices(ConfigureServices())
          .Build()
          .MigrateThenRunAsync();
```

### Options - Mode => AfterAppStart
After the host has started successfully, those `ISeed` classes, that have been set to run `AfterAppStart`
will run in the background, in an `IHostedService`.

### Options - RunAlways => true
You can re-run the `ISeed` procedures every time, when the application starts up. 
Simply set the `RunAlways` property to true in your `ISeed` implementation.

## Example
1. Register your `ISeed` implementations with the `Extensions.RegisterDataSeeder<>`
   * Here goes your implementations of `ISeed`
2. Register this project's tools and services via the `Extensions.RegisterDataSeederServices`
   * It is needed to use this project's tools
3. Replace the `IHost.StartAsync` with `Extensions.MigrateThenRunAsync`
   * It is needed to be able to run the `ISeed` with `BeforeAppStart` option
   * It is needed to ensure the `SeederHistories` table got migrated

_The example is from the `AJProds.EFDataSeeder.Tests.Console` project_

```cs
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
                   collection.AddDbContext<TestDbContext>(builder => builder
                                                             .UseSqlServer(CONNECTION_LOCAL_TEST));
    
                   // EFSeeder setup - with an own EF Migration History table
                   collection.RegisterDataSeederServices(builder =>
                                                         {
                                                             builder
                                                                .UseSqlServer(CONNECTION_LOCAL_TEST);
                                                         });
               };
    }
}
```