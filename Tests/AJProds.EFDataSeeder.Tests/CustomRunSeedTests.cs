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
    public class CustomRunSeedTests : BaseServiceTest
    {
        [Test]
        public async Task CustomStart()
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
                                                        // Ensure the TestDbContext's migration has been run
                                                        await provider.GetRequiredService<TestDbContext>()
                                                                      .Database.EnsureCreatedAsync();

                                                        // Migration will fail due to the inMemory db
                                                        // await provider.GetRequiredService<TestDbContext>()
                                                        //         .Database.MigrateAsync();
                                                    });
            var histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                                 .SeederHistories
                                                 .OrderBy(history => history.LastRunAt)
                                                 .ToList();

            Assert.AreEqual(3, histories.Count);

            // When re-run app
            await SharedServiceProvider.GetRequiredService<BaseSeederManager>()
                                       .SeedAsync();

            // Then
            histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                             .SeederHistories
                                             .OrderBy(history => history.LastRunAt)
                                             .ToList();

            Assert.AreEqual(4, histories.Count);

            Assert.AreEqual("High Prio seed", histories[0].SeedName);
            Assert.False(histories[0].AlwaysRun);

            Assert.AreEqual("Always Run seed", histories[1].SeedName);
            Assert.True(histories[1].AlwaysRun);

            Assert.AreEqual("Low Prio seed", histories[2].SeedName);
            Assert.False(histories[2].AlwaysRun);

            Assert.AreEqual("None seed", histories[3].SeedName);
            Assert.False(histories[3].AlwaysRun);

            var testeeRecords = SharedServiceProvider.GetRequiredService<TestDbContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(4, testeeRecords.Count);

            Assert.True(testeeRecords.Any(t => t.Description == "High Prio seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Always Run seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Low Prio seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "None seed"));
        }
    }
}