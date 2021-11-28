using Microsoft.EntityFrameworkCore;

namespace AJProds.EFDataSeeder.Tests.Console
{
    public class TestDbContext: DbContext
    {
        public const string SCHEMA = "tst";
        
        public DbSet<Testee> Testees { get; set; }
        
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}