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
        public DbSet<MonitoringQueryLog> MonitoringQueryLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Company>(entity =>
            {
                _ = entity.ToTable(nameof(Company), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.Name).HasColumnType("nvarchar").IsRequired().HasMaxLength(64);
            });

            _ = modelBuilder.Entity<Server>(entity =>
            {
                _ = entity.ToTable(nameof(Server), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Name).HasColumnType("nvarchar").IsRequired().HasMaxLength(64);
                _ = entity.Property(e => e.ApiKey).HasColumnType("uniqueidentifier").IsRequired();
                _ = entity.Property(e => e.DoMaintenancePeriod).HasColumnType("smallint").IsRequired();
                _ = entity.Property(e => e.DoMonitoringPeriod).HasColumnType("smallint").IsRequired();

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<JobExecution>(entity =>
            {
                _ = entity.ToTable(nameof(JobExecution), "dbo");
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.ServerFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Type).HasColumnType("smallint").IsRequired();
                _ = entity.Property(e => e.Status).HasColumnType("smallint").IsRequired();
                _ = entity.Property(e => e.CreatedOn).HasColumnType("smalldatetime").IsRequired();
                _ = entity.Property(e => e.CompletedOn).HasColumnType("smalldatetime");
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Server).WithMany().HasForeignKey(e => e.ServerFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                _ = entity.ToTable(nameof(MaintenanceLog), "dbo");
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                _ = entity.Property(e => e.Index).HasColumnType("nvarchar").HasMaxLength(128);
                _ = entity.Property(e => e.Statistic).HasColumnType("nvarchar").HasMaxLength(128);
                _ = entity.Property(e => e.IsError).HasColumnType("bit").IsRequired();
                _ = entity.Property(e => e.ErrorMessage).HasColumnType("nvarchar(max)");
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobServerLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringJobServerLog), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128);
                _ = entity.Property(e => e.Priority).HasColumnType("tinyint");
                _ = entity.Property(e => e.CheckId).HasColumnType("int").IsRequired();
                _ = entity.Property(e => e.Details).HasColumnType("nvarchar(max)");

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobServerWaitStatLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringJobServerWaitStatLog), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Type).HasColumnType("nvarchar").HasMaxLength(60);
                _ = entity.Property(e => e.TimeInMs).HasColumnType("bigint");

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobServerFindingLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringJobServerFindingLog), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Priority).HasColumnType("tinyint");
                _ = entity.Property(e => e.CheckId).HasColumnType("int").IsRequired();
                _ = entity.Property(e => e.Details).HasColumnType("nvarchar(max)");

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobIndexMissingLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringJobIndexMissingLog), "dbo");
                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.DatabaseName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                _ = entity.Property(e => e.TableName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
                _ = entity.Property(e => e.MagicBenefitNumber).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.Impact).HasColumnType("nvarchar(max").IsRequired();
                _ = entity.Property(e => e.IndexDetails).HasColumnType("nvarchar(max)").IsRequired();
                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringJobIndexUsageLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringJobIndexUsageLog), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();

                _ = entity.Property(e => e.CompanyFk).IsRequired().HasColumnType("bigint");
                _ = entity.Property(e => e.JobFk).IsRequired().HasColumnType("bigint");

                _ = entity.Property(e => e.DatabaseName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                _ = entity.Property(e => e.IsClusteredIndex).IsRequired().HasColumnType("bit");
                _ = entity.Property(e => e.IndexName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                _ = entity.Property(e => e.TableName).IsRequired().HasMaxLength(128).HasColumnType("nvarchar");
                _ = entity.Property(e => e.IndexDetails).IsRequired().HasColumnType("nvarchar(max)");
                _ = entity.Property(e => e.Usage).IsRequired().HasColumnType("nvarchar(max)");

                _ = entity.Property(e => e.ReadsUsage).IsRequired().HasColumnType("bigint");
                _ = entity.Property(e => e.WriteUsage).IsRequired().HasColumnType("bigint");

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });

            _ = modelBuilder.Entity<MonitoringQueryLog>(entity =>
            {
                _ = entity.ToTable(nameof(MonitoringQueryLog), "dbo");

                _ = entity.HasKey(e => e.Id);
                _ = entity.Property(e => e.Id).HasColumnType("bigint").ValueGeneratedOnAdd();

                _ = entity.Property(e => e.CompanyFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.JobFk).HasColumnType("bigint").IsRequired();
                _ = entity.Property(e => e.QueryLogSortBy).IsRequired().HasColumnType("tinyint");
                _ = entity.Property(e => e.DatabaseName).IsRequired().HasColumnType("nvarchar(128)");
                _ = entity.Property(e => e.QueryPlanCost).HasColumnType("float");
                _ = entity.Property(e => e.QueryText).HasColumnType("nvarchar(max)");
                _ = entity.Property(e => e.Warnings).HasColumnType("varchar(max)");
                _ = entity.Property(e => e.QueryPlan).HasColumnType("xml");
                _ = entity.Property(e => e.MissingIndexes).HasColumnType("nvarchar(max)");
                _ = entity.Property(e => e.ImplicitConversionInfo).HasColumnType("nvarchar(max)");
                _ = entity.Property(e => e.ExecutionCount).HasColumnType("bigint");
                _ = entity.Property(e => e.ExecutionsPerMinute).HasColumnType("money");
                _ = entity.Property(e => e.TotalCpu).HasColumnType("bigint");
                _ = entity.Property(e => e.AverageCpu).HasColumnType("bigint");
                _ = entity.Property(e => e.TotalDuration).HasColumnType("bigint");
                _ = entity.Property(e => e.AverageDuration).HasColumnType("bigint");
                _ = entity.Property(e => e.TotalReads).HasColumnType("bigint");
                _ = entity.Property(e => e.AverageReads).HasColumnType("bigint");
                _ = entity.Property(e => e.TotalReturnedRows).HasColumnType("bigint");
                _ = entity.Property(e => e.AverageReturnedRows).HasColumnType("money");
                _ = entity.Property(e => e.MinReturnedRows).HasColumnType("bigint");
                _ = entity.Property(e => e.MaxReturnedRows).HasColumnType("bigint");
                _ = entity.Property(e => e.NumberOfPlans).HasColumnType("int");
                _ = entity.Property(e => e.NumberOfDistinctPlans).HasColumnType("int");
                _ = entity.Property(e => e.LastExecutionTime).HasColumnType("datetime");
                _ = entity.Property(e => e.QueryHash).HasColumnType("binary(8)").IsRequired(false);

                _ = entity.HasOne(e => e.Company).WithMany().HasForeignKey(e => e.CompanyFk).OnDelete(DeleteBehavior.Cascade);
                _ = entity.HasOne(e => e.Job).WithMany().HasForeignKey(e => e.JobFk).OnDelete(DeleteBehavior.Cascade);
            });
        }

        public void SeedData()
        {
            var companyExists = Companies.Any(c => c.Name == "SqlSenseiTest");

            if (!companyExists)
            {
                _ = Companies.Add(new Company("SqlSenseiTest"));
            }

            var serverExists = Servers.Any(s => s.Name == "SqlSenseiTestServer");

            if (!serverExists)
            {
                _ = Servers.Add(new Server
                {
                    Name = "SqlSenseiTestServer",
                    ApiKey = Guid.Parse("fd2639d8-11cb-4e12-b93e-580975ee5531"),
                    CompanyFk = 1,
                    DoMaintenancePeriod = Core.SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM,
                    DoMonitoringPeriod = Core.SqlSenseiRunMonitoringPeriod.Every15Minutes
                });
            }

            _ = SaveChanges();
        }
    }
}
