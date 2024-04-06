using Microsoft.EntityFrameworkCore;

namespace SqlSensei.Api.Storage
{
    public class SqlSenseiDbContext(DbContextOptions<SqlSenseiDbContext> options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<MonitoringJobIndexMissingLog> MonitoringJobIndexMissingLogs { get; set; }
        public DbSet<MonitoringJobIndexUsageLog> MonitoringJobIndexUsageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Company>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).ValueGeneratedOnAdd();
                _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
                _ = entity.Property(e => e.ApiKey).IsRequired();
            });

            _ = modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.Index).HasMaxLength(128);
                _ = entity.Property(e => e.Statistic).HasMaxLength(128);
                _ = entity.Property(e => e.IsError).IsRequired();
                _ = entity.Property(e => e.ErrorMessage);
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobIndexMissingLog>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.TableName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.Impact).IsRequired();
                _ = entity.Property(e => e.IndexDetails).IsRequired();
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobIndexUsageLog>(entity =>
            {
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.IndexName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.TableName).IsRequired().HasMaxLength(128);
                _ = entity.Property(e => e.IndexDetails).IsRequired();
                _ = entity.Property(e => e.Usage).IsRequired();
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
