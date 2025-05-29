using DataImport.Conversion;
using Microsoft.EntityFrameworkCore;

namespace DataImport.Dal;

public class DataImportDbContext : DbContext
{
    public DbSet<PrecipitationRecord> PrecipitationRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=dataimport.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrecipitationRecord>()
            .HasKey(r => r.Id);
    }
}