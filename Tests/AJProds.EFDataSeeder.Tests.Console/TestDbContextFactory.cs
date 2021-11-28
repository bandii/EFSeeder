using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AJProds.EFDataSeeder.Tests.Console
{
    /// <inheritdoc />
    public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        /// <inheritdoc />
        public TestDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseSqlServer(Program.CONNECTION_LOCAL_TEST);

            return new TestDbContext(optionsBuilder.Options);
        }
    }
}