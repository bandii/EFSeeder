using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Core.Db;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using AJProds.EFDataSeeder.Tests.Common;
using AJProds.EFDataSeeder.Tests.Common.AfterAppStartSeed;

namespace AJProds.EFDataSeeder.Tests
{
    [Category("Smoke")]
    public class StartThenSeedTests : BaseServiceTest
    {
        protected override void RegisterServicesOnSetUp()
        {
            base.RegisterServicesOnSetUp();
            
            SharedServiceCollection.AddTransient(_ => Mock.Of<ILogger<SeederHostedService>>());
        }

        [Test]
        public async Task SeedsInvokedInOrder()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<HighPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LowPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<AlwaysRunTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<NoneTestSeed>();
            
            var testee = SharedServiceProvider.GetRequiredService<IHostedService>();

            // When
            await testee.StartAsync(default);

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

            var testeeRecords = SharedServiceProvider.GetRequiredService<TestMSSQLDbContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(3, testeeRecords.Count);

            Assert.True(testeeRecords.Any(t => t.Description == "High Prio seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Always Run seed"));
            Assert.True(testeeRecords.Any(t => t.Description == "Low Prio seed"));
        }

        [Test]
        public async Task CancelStartInvoked()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<HighPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LowPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<AlwaysRunTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<NoneTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LengthyHighTestSeed>();
            
            var testee = SharedServiceProvider.GetRequiredService<IHostedService>();

            // When
            await Task.Run(() =>
                           {
                               var cts = new CancellationTokenSource();
                               testee.StartAsync(cts.Token);
                               cts.Cancel();
                           });

            // Then
            var histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                             .SeederHistories
                                             .OrderBy(history => history.LastRunAt)
                                             .ToList();

            Assert.AreEqual(0, histories.Count);

            var testeeRecords = SharedServiceProvider.GetRequiredService<TestMSSQLDbContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(0, testeeRecords.Count);
        }

        [Test]
        public async Task StopInvoked()
        {
            // Given
            // Register seeders
            SharedServiceCollection.RegisterDataSeeder<HighPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LowPrioTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<AlwaysRunTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<NoneTestSeed>();
            SharedServiceCollection.RegisterDataSeeder<LengthyHighTestSeed>();
            
            var testee = SharedServiceProvider.GetRequiredService<IHostedService>();

            // When
            await Task.Run(() =>
                           {
                               var cts = new CancellationTokenSource();
                               testee.StartAsync(cts.Token);
                               testee.StopAsync(default);
                           });

            // Then
            var histories = SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                             .SeederHistories
                                             .OrderBy(history => history.LastRunAt)
                                             .ToList();

            Assert.AreEqual(0, histories.Count);

            var testeeRecords = SharedServiceProvider.GetRequiredService<TestMSSQLDbContext>()
                                                     .Testees
                                                     .ToList();

            Assert.AreEqual(0, testeeRecords.Count);
        }
    }
}