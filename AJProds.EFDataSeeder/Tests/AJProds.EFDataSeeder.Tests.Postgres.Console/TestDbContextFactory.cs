using AJProds.EFDataSeeder.Tests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AJProds.EFDataSeeder.Tests.Console;

/// <inheritdoc />
public class TestDbContextFactory : IDesignTimeDbContextFactory<TestPostgreSQLDbContext>
{
    /// <inheritdoc />
    public TestPostgreSQLDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestPostgreSQLDbContext>();
        optionsBuilder.UseNpgsql(Program.ConnectionPostgresTest);
    
        return new TestPostgreSQLDbContext(optionsBuilder.Options);
    }
}