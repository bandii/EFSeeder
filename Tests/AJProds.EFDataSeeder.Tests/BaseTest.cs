using System;

using Microsoft.Extensions.DependencyInjection;

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
    }
}