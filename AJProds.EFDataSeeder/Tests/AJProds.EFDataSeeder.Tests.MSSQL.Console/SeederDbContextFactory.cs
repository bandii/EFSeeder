using AJProds.EFDataSeeder.Core.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AJProds.EFDataSeeder.Tests.Console;

/// <inheritdoc />
public class SeederDbContextFactory : IDesignTimeDbContextFactory<SeederDbContext>
{
    /// <inheritdoc />
    public SeederDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SeederDbContext>();
        optionsBuilder.UseSqlServer(Program.ConnectionMssqlTest, 
                                    x => x.MigrationsAssembly(typeof(MsSql.Extensions).Assembly.FullName));

        return new SeederDbContext(optionsBuilder.Options);
    }
}