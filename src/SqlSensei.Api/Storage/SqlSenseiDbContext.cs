using Microsoft.EntityFrameworkCore;

namespace SqlSensei.Api.Storage
{
    public class SqlSenseiDbContext(DbContextOptions<SqlSenseiDbContext> options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<JobExecution> Jobs { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<MonitoringJobServerLog> MonitoringJobServerLogs { get; set; }
        public DbSet<MonitoringJobServerWaitStatLog> MonitoringJobServerWaitStatLogs { get; set; }
        public DbSet<MonitoringJobServerFindingLog> MonitoringJobServerFindingLogs { get; set; }
        public DbSet<MonitoringJobIndexMissingLog> MonitoringJobIndexMissingLogs { get; set; }
        public DbSet<MonitoringJobIndexUsageLog> MonitoringJobIndexUsageLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable(nameof(Company), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.Name).HasColumnType("nvarchar").IsRequired().HasMaxLength(64);
            });

            _ = modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable(nameof(Server), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Name).HasColumnType("nvarchar").IsRequired().HasMaxLength(64);
                _ = entity.Property(e => e.ApiKey).HasColumnType("uniqueidentifier").IsRequired();
                _ = entity.Property(e => e.DoMaintenancePeriod).HasColumnType("smallint").IsRequired();
                _ = entity.Property(e => e.DoMonitoringPeriod).HasColumnType("smallint").IsRequired();

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<JobExecution>(entity =>
            {
                entity.ToTable(nameof(JobExecution), "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.ServerFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.Type).HasColumnType("smallint").IsRequired();
                entity.Property(e => e.Status).HasColumnType("smallint").IsRequired();
                entity.Property(e => e.CreatedOn).HasColumnType("smalldatetime").IsRequired();
                entity.Property(e => e.CompletedOn).HasColumnType("smalldatetime");
                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Server).WithMany().HasForeignKey(e => e.ServerFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.ToTable(nameof(MaintenanceLog), "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                entity.Property(e => e.Index).HasColumnType("nvarchar").HasMaxLength(128);
                entity.Property(e => e.Statistic).HasColumnType("nvarchar").HasMaxLength(128);
                entity.Property(e => e.IsError).HasColumnType("bit").IsRequired();
                entity.Property(e => e.ErrorMessage).HasColumnType("nvarchar(max)");
                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MonitoringJobServerLog>(entity =>
            {
                entity.ToTable(nameof(MonitoringJobServerLog), "dbo");
                
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128);
                entity.Property(e => e.Priority).HasColumnType("tinyint");
                entity.Property(e => e.CheckId).HasColumnType("int").IsRequired();
                entity.Property(e => e.Details).HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MonitoringJobServerWaitStatLog>(entity =>
            {
                entity.ToTable(nameof(MonitoringJobServerWaitStatLog), "dbo");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.Type).HasColumnType("nvarchar").HasMaxLength(60);
                entity.Property(e => e.TimeInMs).HasColumnType("bigint");

                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MonitoringJobServerFindingLog>(entity =>
            {
                entity.ToTable(nameof(MonitoringJobServerFindingLog), "dbo");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.Priority).HasColumnType("tinyint");
                entity.Property(e => e.CheckId).HasColumnType("int").IsRequired();
                entity.Property(e => e.Details).HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MonitoringJobIndexMissingLog>(entity =>
            {
                entity.ToTable(nameof(MonitoringJobIndexMissingLog), "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                entity.Property(e => e.TableName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                entity.Property(e => e.MagicBenefitNumber).HasColumnType("bigint").IsRequired();
                entity.Property(e => e.Impact).HasColumnType("nvarchar(max").IsRequired();
                entity.Property(e => e.IndexDetails).HasColumnType("nvarchar(max)").IsRequired();
                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MonitoringJobIndexUsageLog>(entity =>
            {
                entity.ToTable(nameof(MonitoringJobIndexUsageLog), "dbo");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();

                entity.Property(e => e.CompanyFk).IsRequired().HasColumnType("bigint");
                entity.Property(e => e.JobFk).IsRequired().HasColumnType("bigint");

                entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                entity.Property(e => e.IsClusteredIndex).IsRequired().HasColumnType("bit");
                entity.Property(e => e.IndexName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                entity.Property(e => e.TableName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                entity.Property(e => e.IndexDetails).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.Usage).IsRequired().HasColumnType("nvarchar(max)");

                entity.Property(e => e.ReadsUsage).IsRequired().HasColumnType("bigint");
                entity.Property(e => e.WriteUsage).IsRequired().HasColumnType("bigint");

                entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });
        }

        public void SeedData()
        {
            var companyExists = Companies.Any(c => c.Name == "SqlSenseiTest");

            if (!companyExists)
            {
                Companies.Add(new Company ("SqlSenseiTest"));
            }

            var serverExists = Servers.Any(s => s.Name == "SqlSenseiTestServer");

            if (!serverExists)
            {
                Servers.Add(new Server
                {
                    Name = "SqlSenseiTestServer",
                    ApiKey = Guid.Parse("fd2639d8-11cb-4e12-b93e-580975ee5531"),
                    CompanyFk = 1,
                    DoMaintenancePeriod = Core.SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM,
                    DoMonitoringPeriod = Core.SqlSenseiRunMonitoringPeriod.Every15Minutes
                });
            }

            SaveChanges();
        }
    }
}
