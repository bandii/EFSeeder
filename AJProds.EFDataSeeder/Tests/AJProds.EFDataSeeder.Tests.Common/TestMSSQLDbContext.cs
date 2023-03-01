using Microsoft.EntityFrameworkCore;

namespace AJProds.EFDataSeeder.Tests.Common;

public class TestMSSQLDbContext : DbContext, ITestContext
{
    public DbSet<Testee> Testees { get; set; }

    public TestMSSQLDbContext(DbContextOptions<TestMSSQLDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(ITestContext.SCHEMA);

        modelBuilder.Entity<Testee>(builder => builder.Property(testee => testee.Description)
                                                      .HasMaxLength(100));

        base.OnModelCreating(modelBuilder);
    }
}