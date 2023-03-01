using AJProds.EFDataSeeder.Tests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AJProds.EFDataSeeder.Tests.Console;

/// <inheritdoc />
public class TestDbContextFactory : IDesignTimeDbContextFactory<TestMSSQLDbContext>
{
    /// <inheritdoc />
    public TestMSSQLDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestMSSQLDbContext>();
        optionsBuilder.UseSqlServer(Constants.ConnectionMssqlTest);
    
        return new TestMSSQLDbContext(optionsBuilder.Options);
    }
}