using Microsoft.EntityFrameworkCore;

namespace AJProds.EFDataSeeder.Core.Db
{
    public class SeederDbContext : DbContext
    {
        public const string SCHEMA = "sdr";

        public DbSet<SeederHistory> SeederHistories { get; set; }

        public SeederDbContext(DbContextOptions<SeederDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Options.HistoryTableConfiguration != null)
            {
                modelBuilder.ApplyConfiguration(Options.HistoryTableConfiguration);
                
                return;
            }
            
            modelBuilder.HasDefaultSchema(SCHEMA);

            modelBuilder.Entity<SeederHistory>(builder =>
                                               {
                                                   builder.HasIndex(history => history.SeedName)
                                                          .IsUnique();

                                                   builder.Property(history => history.SeedName)
                                                          .IsRequired();
                                               });

            base.OnModelCreating(modelBuilder);
        }
    }
}