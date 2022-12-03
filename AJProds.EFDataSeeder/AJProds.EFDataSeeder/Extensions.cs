using System;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Core.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJProds.EFDataSeeder
{
    /// <summary>
    /// Wrapper around the Core services
    /// </summary>
    public static class Extensions
    {
        /// <inheritdoc cref="Core.Extensions.RegisterDataSeederServices"/>
        public static IServiceCollection RegisterDataSeederServices(this IServiceCollection collection,
                                                                    Action<DbContextOptionsBuilder> dbConnectionOptions,
                                                                    IEntityTypeConfiguration<SeederHistory> historyTableConfiguration = null)
        {
            return AJProds.EFDataSeeder.Core.Extensions.RegisterDataSeederServices(collection,
                                                                                   dbConnectionOptions,
                                                                                   historyTableConfiguration);
        }

        /// <inheritdoc cref="Core.Extensions.RegisterDataSeeder{TSeeder}"/>
        public static IServiceCollection RegisterDataSeeder<TSeeder>(this IServiceCollection collection)
            where TSeeder : class, ISeed
        {
            return AJProds.EFDataSeeder.Core.Extensions.RegisterDataSeeder<TSeeder>(collection);
        }

        /// <inheritdoc cref="Core.Extensions.MigrateThenRunAsync"/>
        public static Task MigrateThenRunAsync(this IHost host,
                                                     Func<IServiceProvider, Task> actionBeforeRun = null)
        {
            return AJProds.EFDataSeeder.Core.Extensions.MigrateThenRunAsync(host, actionBeforeRun);
        }
    }
}