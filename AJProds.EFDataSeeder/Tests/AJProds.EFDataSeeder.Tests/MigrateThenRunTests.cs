using System.Linq;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Core.Db;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using NUnit.Framework;

using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Common.BeforeAppStartSeed;

namespace AJProds.EFDataSeeder.Tests
{
    [Category("Smoke")]
    public class MigrateThenRunTests : BaseServiceTest
    {
        [Test]
        public async Task SeedsInvokedInOrder()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<HighPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LowPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<AlwaysRunTestSeed>();

            var testee = new Mock<IHost>();
            testee.Setup(host => host.Services)
                  .Returns(SharedServiceProvider);

            // When
            await testee.Object.MigrateThenRunAsync(async provider =>
                                                    {
                                                        // Ensure the ITestContext's migration has been run
                                                        await provider.GetRequiredService<ITestContext>()
                                                                      .Database.EnsureCreatedAsync();

                                                        // Migration will fail due to the inMemory db
                                                        // await provider.GetRequiredService<ITestContext>()
                                                        //         .Database.MigrateAsync();
                                                    });

            // Then
            var histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                                 .SeederHistories
                                                 .OrderBy(history => history.LastRunAt)
                                                 .ToList();

            Assert.AreEqual(3, histories.Count);

            Assert.AreEqual("High Prio seed", histories[0].SeedName);
            Assert.False(histories[0].AlwaysRun);

            Assert.AreEqual("Always Run seed", histories[1].SeedName);
            Assert.True(histories[1].AlwaysRun);

            Assert.AreEqual("Low Prio seed", histories[2].SeedName);
            Assert.False(histories[2].AlwaysRun);

            var testeeRecords = SharedServiceProvider.GetRequiredService<ITestContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(3, testeeRecords.Count);

            Assert.True(testeeRecords.Any(t => t.Description == "High Prio seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Always Run seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Low Prio seed"));
        }

        [Test]
        public async Task AlwaysRunUpdatesHistory()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<HighPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LowPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<AlwaysRunTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<NoneTestSeed>();

            var testee = new Mock<IHost>();
            testee.Setup(host => host.Services)
                  .Returns(SharedServiceProvider);

            await testee.Object.MigrateThenRunAsync(async provider =>
                                                    {
                                                        // Ensure the ITestContext's migration has been run
                                                        await provider.GetRequiredService<ITestContext>()
                                                                      .Database.EnsureCreatedAsync();

                                                        // Migration will fail due to the inMemory db
                                                        // await provider.GetRequiredService<ITestContext>()
                                                        //         .Database.MigrateAsync();
                                                    });

            var histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                                 .SeederHistories
                                                 .OrderBy(history => history.LastRunAt)
                                                 .ToList();

            var lastRunAtHigh = histories[0].LastRunAt;
            var lastRunAtAlwaysRun = histories[1].LastRunAt;
            var lastRunAtLow = histories[2].LastRunAt;

            // When re-run app
            await testee.Object.MigrateThenRunAsync(async provider =>
                                                    {
                                                        // Ensure the ITestContext's migration has been run
                                                        await provider.GetRequiredService<ITestContext>()
                                                                      .Database.EnsureCreatedAsync();

                                                        // Migration will fail due to the inMemory db
                                                        // await provider.GetRequiredService<ITestContext>()
                                                        //         .Database.MigrateAsync();
                                                    });

            // Then
            histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                             .SeederHistories
                                             .OrderBy(history => history.LastRunAt)
                                             .ToList();

            Assert.AreEqual(3, histories.Count);

            Assert.AreEqual("High Prio seed", histories[0].SeedName);
            Assert.False(histories[0].AlwaysRun);
            Assert.AreEqual(lastRunAtHigh, histories[0].LastRunAt);

            Assert.AreEqual("Low Prio seed", histories[1].SeedName);
            Assert.False(histories[1].AlwaysRun);
            Assert.AreEqual(lastRunAtLow, histories[1].LastRunAt);

            Assert.AreEqual("Always Run seed", histories[2].SeedName);
            Assert.True(histories[2].AlwaysRun);
            Assert.Greater(histories[2].LastRunAt, lastRunAtAlwaysRun);

            var testeeRecords = SharedServiceProvider.GetRequiredService<ITestContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(4, testeeRecords.Count);

            Assert.True(testeeRecords.Any(t => t.Description == "High Prio seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Low Prio seed"));
            Assert.AreEqual(2, testeeRecords.Count(t => t.Description == "Always Run seed"));
        }
    }
}