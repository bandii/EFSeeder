using System;
using System.Threading;
using System.Threading.Tasks;

using AJProds.EFDataSeeder.Core;
using AJProds.EFDataSeeder.Core.Db;
using AJProds.EFDataSeeder.MsSql.Db.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AJProds.EFDataSeeder.MsSql
{
    /// <summary>
    /// Wrapper around the Core services
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Registers all the necessary tools to be able to use EFDataSeeder
        /// </summary>
        /// <param name="collection"><see cref="IServiceCollection"/></param>
        /// <param name="connectionString">The connection string</param>
        /// <param name="historyTableConfiguration">Provide your own <see cref="SeederHistory"/> configuration</param>
        /// <param name="schemaName">The name of the schema, what the nuget will use</param>
        public static IServiceCollection RegisterDataSeederServices(this IServiceCollection collection,
                                                                    string connectionString,
                                                                    IEntityTypeConfiguration<SeederHistory> historyTableConfiguration = null,
                                                                    string schemaName = "sdr")
        {
            return AJProds.EFDataSeeder.Core.Extensions
                          .RegisterDataSeederServices(collection,
                                                      options =>
                                                      {
                                                          options.UseSqlServer(connectionString,
                                                                               x => x.MigrationsAssembly(typeof(SeederDbContextModelSnapshot).Assembly.FullName)
                                                                              );
                                                      },
                                                      historyTableConfiguration,
                                                      schemaName);
        }

        /// <inheritdoc cref="Core.Extensions.RegisterDataSeeder{TSeeder}"/>
        public static IServiceCollection RegisterDataSeeder<TSeeder>(this IServiceCollection collection)
            where TSeeder : class, ISeed
        {
            return AJProds.EFDataSeeder.Core.Extensions.RegisterDataSeeder<TSeeder>(collection);
        }

        /// <inheritdoc cref="Core.Extensions.MigrateThenRunAsync"/>
        public static Task MigrateThenRunAsync(this IHost host,
                                               Func<IServiceProvider, Task> actionBeforeRun = null,
                                               CancellationToken cts = default)
        {
            return AJProds.EFDataSeeder.Core.Extensions.MigrateThenRunAsync(host, actionBeforeRun, cts);
        }
    }
}