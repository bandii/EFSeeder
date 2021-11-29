using Microsoft.EntityFrameworkCore;

namespace AJProds.EFDataSeeder.Tests.Common
{
    public class TestDbContext : DbContext
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

            modelBuilder.Entity<Testee>(builder => builder.Property(testee => testee.Description)
                                                          .HasMaxLength(100));

            base.OnModelCreating(modelBuilder);
        }
    }
}