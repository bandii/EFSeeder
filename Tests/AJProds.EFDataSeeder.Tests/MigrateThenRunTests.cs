using System.Linq;

using AJProds.EFDataSeeder.Db;
using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Console;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace AJProds.EFDataSeeder.Tests
{
    [Category("MigrateThenRunAsync")]
    public class MigrateThenRunTests : BaseServiceTest
    {
        [SetUp]
        public void SetUp()
        {
            // Tests logger
            SharedServiceCollection.AddTransient(_ => Mock.Of<ILogger>());
            SharedServiceCollection.AddTransient(_ => Mock.Of<ILogger<BaseSeederManager>>());

            // Add TestDbContext
            SharedServiceCollection.AddDbContext<TestDbContext>(builder => builder
                                                                   .UseInMemoryDatabase("MigrateThenRunTests"));

            // Register the seeder tools
            SharedServiceCollection.RegisterDataSeederServices(builder => builder
                                                                  .UseInMemoryDatabase("MigrateThenRunTests"));

            // MigrateThenRunAsync uses a scope
            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(SharedServiceProvider);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory.Setup(x => x.CreateScope())
                               .Returns(serviceScope.Object);

            SharedServiceCollection.AddTransient(_ => serviceScopeFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Clear();
        }

        [Test]
        public void Test1()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<TestSeed>();
            SharedServiceCollection.RegisterDataSeeder<NewerTestSeed>();

            var testee = new Mock<IHost>();
            testee.Setup(host => host.Services)
                  .Returns(SharedServiceProvider);

            // When
            testee.Object.MigrateThenRunAsync(async provider =>
                                              {
                                                  // Ensure the TestDbContext's migration has been run
                                                  await provider.GetRequiredService<TestDbContext>()
                                                                .Database.EnsureCreatedAsync();

                                                  // Migration will fail due to the inMemory db
                                                  // await provider.GetRequiredService<TestDbContext>()
                                                  //         .Database.MigrateAsync();
                                              });

            // Then
            var history = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                           .SeederHistories
                                           .Single();

            Assert.AreEqual("Low Prio seed", history.SeedName);
            Assert.False(history.AlwaysRun);
            
            var testeeRecord = SharedServiceProvider.GetRequiredService<TestDbContext>()
                                           .Testees
                                           .Single();

            Assert.AreEqual("Low Prio seed", history.SeedName);
            Assert.False(history.AlwaysRun);
        }
    }
}