using AJProds.EFDataSeeder.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AJProds.EFDataSeeder.MsSql;

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
    /// <param name="schemaName">The name of the schema, what the nuget will use</param>
    public static IServiceCollection RegisterDataSeederServices(this IServiceCollection collection,
                                                                string connectionString,
                                                                string schemaName = "sdr")
    {
        return collection.RegisterDataSeederServices(options =>
                                                     {
                                                         options.UseSqlServer(connectionString,
                                                                              x => x.MigrationsAssembly(typeof(Extensions).Assembly.FullName)
                                                                             );
                                                     },
                                                     schemaName);
    }
}