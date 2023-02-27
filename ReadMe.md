## Problem
There might be situations, when you need to seed some data with keeping the business logic in mind.
Applying business logic and versioning of SQL migration scripts can be time-consuming. 
We should not forget about it would be great to be able to trace back the changes done in the database.

### Examples
* You got a legacy project with a db schema you know you will need to modify during your upcoming implementations. 
You also know that, you will need to move some data from one table to another by applying business logic to it just before 
the app starts up for the first time with the latest migrations.
* You have a missing feature in your system, and you have a repetitive data manipulation (inject) task. 
For example, you need to modify user access controlled via db tables, because you do not have a UI, feature in the app to do so.
You also would like to apply the business logic regarding the User Access of the project and you also would like to be able to trace back 
who modified user data and when?

_Please note that you should not face with these problems. Instead of trying to patch problems like this, 
it worth to plan ahead and prepare for these kind of problems in the start. You should also check EF Core's data-seeding option: https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding_

## Solution
This package helps you seeding data by accessing [the IoC container](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0) of the application.

You can seed the data with using the existing business logic already composed in your solution and access it later in the IoC of your app.
All you need to do is simply to register ISeed implementations to your [ServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollection?view=dotnet-plat-ext-6.0). 

Later on, you can see which seeds have been run via the `sdr.SeederHistories` table.

## How to use?
0. Either generate the migrations for your DB, or reference the existing, db-specific nuget packages of this project.
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
    public const string CONNECTION_STRING =
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
                   collection.RegisterDataSeeder<AlwaysRunTestSeed>();
                   
                   // My own, custom, test setup
                   collection.AddDbContext<TestDbContext>(builder => builder
                                                             .UseSqlServer(CONNECTION_STRING));
    
                   // EFSeeder setup - with an own EF Migration History table
                   collection.RegisterDataSeederServices(CONNECTION_STRING);
               };
    }
    
    /// <summary>
    /// My custom seed, that will run after all app start
    /// </summary>
    public class AlwaysRunTestSeed: ISeed
    {
        private readonly TestDbContext _dbContext;

        public int Priority => 50;

        public string SeedName => "Always Run seed";

        public SeedMode Mode => SeedMode.AfterAppStart;

        public bool AlwaysRun => true;

        public AlwaysRunTestSeed(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SeedAsync()
        {
            _dbContext.Testees.Add(new Testee
                                   {
                                       Description = "Always Run seed"
                                   });

            await _dbContext.SaveChangesAsync();
        }
    }
}
```

## Seed options

### SeadMode.None
No `ISeed` logic will be run. You need to find the `BaseSeederManager` in your ioc
and run it with the `SeedMode.None` argument.
```cs
var baseSeederManager = provider.GetRequiredService<BaseSeederManager>();
baseSeederManager.Seed(SeedMode.None);
```

### SeadMode.BeforeAppStart
You must use the `Extensions.MigrateThenRunAsync` with your `HostBuilder`,
because this procedure will not start up the host until all migration
and seed (with _`SeedMode.BeforeAppStart`_) has been run successfully.

```cs
await Host.CreateDefaultBuilder()
          .ConfigureServices(ConfigureServices())
          .Build()
          .MigrateThenRunAsync();
```

### SeadMode.AfterAppStart
After the host has started successfully, those `ISeed` classes, that have been set to run `AfterAppStart`
will run in the background, in an `IHostedService`.

### ISeed.RunAlways => true
You can re-run the `ISeed` procedures every time, when the application starts up.
Simply set the `RunAlways` property to true in your `ISeed` implementation.

# TODO
- [ ] Bump versions to LTS -> NET 6
- [ ] Add UI in a new project?