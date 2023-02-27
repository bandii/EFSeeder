using AJProds.EFDataSeeder.Core.Db;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AJProds.EFDataSeeder.Tests.Console
{
    /// <inheritdoc />
    public class SeederDbContextFactory : IDesignTimeDbContextFactory<SeederDbContext>
    {
        /// <inheritdoc />
        public SeederDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SeederDbContext>();
            optionsBuilder.UseSqlServer(Program.CONNECTION_LOCAL_TEST);

            return new SeederDbContext(optionsBuilder.Options);
        }
    }
}