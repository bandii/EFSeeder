using System;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Core.Db;
using AJProds.EFDataSeeder.Tests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

namespace AJProds.EFDataSeeder.Tests
{
    /// <summary>
    /// The goal of this class is to share the common objects via static properties
    /// </summary>
    [TestFixture]
    public abstract class BaseServiceTest
    {
        private IServiceCollection _serviceCollection;

        /// <summary>
        /// Register and modify your services here
        /// </summary>
        protected IServiceCollection SharedServiceCollection
        {
            get => _serviceCollection ??= new ServiceCollection();
            private set => _serviceCollection = value;
        }

        /// <summary>
        /// Access your services here. The provider will be re-created every time,
        /// so you can register your services anytime
        /// </summary>
        protected IServiceProvider SharedServiceProvider
            => SharedServiceCollection.BuildServiceProvider();

        /// <summary>
        /// Clears the services already registered
        /// </summary>
        protected void Clear()
        {
            SharedServiceCollection = null;
        }

        [SetUp]
        public void SetUp()
        {
            // Tests logger
            RegisterServicesOnSetUp();

            // Add TestMSSQLDbContext
            SharedServiceCollection.AddDbContext<ITestContext, TestMSSQLDbContext>(builder => builder
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

        protected virtual void RegisterServicesOnSetUp()
        {
            SharedServiceCollection.AddTransient(_ => Mock.Of<ILogger>());
            SharedServiceCollection.AddTransient(_ => Mock.Of<ILogger<BaseSeederManager>>());
        }

        [TearDown]
        public void TearDown()
        {
            SharedServiceProvider.GetRequiredService<SeederDbContext>()
                                 .Database
                                 .EnsureDeleted();

            SharedServiceProvider.GetRequiredService<ITestContext>()
                                 .Database
                                 .EnsureDeleted();

            Clear();
        }
    }
}