using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.Services
{
    public class JobService(SqlSenseiDbContext dbContext, CurrentCompanyService company)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;
        public CurrentCompanyService Company { get; } = company;

        public async Task<Result<CanExecuteJobsResponse>> CanExecuteJobs()
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<CanExecuteJobsResponse>(companyResult);
            }

            var jobsMaintenanceForCompany = await DbContext.Jobs
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Type == JobType.Maintenance)
                .OrderByDescending(x => x.CreatedOn)
                .Take(10)
                .ToListAsync();

            var jobsMonitoringForCompany = await DbContext.Jobs
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Type == JobType.MonitoringIndex)
                .OrderByDescending(x => x.CreatedOn)
                .Take(10)
                .ToListAsync();

            var canExecuteMaintenance = false;

            if (!jobsMaintenanceForCompany.Any())
            {
                canExecuteMaintenance = true;
            }

            if (jobsMaintenanceForCompany.Any() && DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday && DateTime.UtcNow.Hour == 6)
            {
                var lastMaintenanceJobDate = jobsMaintenanceForCompany.First().CreatedOn;
                var weeksSinceLastJob = (DateTime.UtcNow - lastMaintenanceJobDate).TotalDays / 7;

                switch (companyResult.Value.CurrentServer.DoMaintenancePeriod)
                {
                    case SqlSenseiRunMaintenancePeriod.EveryWeekendSundayAt6AM:
                        canExecuteMaintenance = weeksSinceLastJob >= 1;
                        break;
                    case SqlSenseiRunMaintenancePeriod.EveryOtherWeekendSundayAt6AM:
                        canExecuteMaintenance = weeksSinceLastJob >= 2;
                        break;
                    case SqlSenseiRunMaintenancePeriod.Never:
                        canExecuteMaintenance = false;
                        break;
                }
            }

            var canExecuteMonitoring = false;

            if (!jobsMonitoringForCompany.Any())
            {
                canExecuteMonitoring = true;
            }

            if (jobsMonitoringForCompany.Any())
            {
                var lastMonitoringJobDate = jobsMonitoringForCompany.First().CreatedOn;
                var minutesSinceLastJob = (DateTime.UtcNow - lastMonitoringJobDate).TotalMinutes;

                switch (companyResult.Value.CurrentServer.DoMonitoringPeriod)
                {
                    case SqlSenseiRunMonitoringPeriod.Every15Minutes:
                        canExecuteMonitoring = minutesSinceLastJob >= 15;
                        break;
                    case SqlSenseiRunMonitoringPeriod.Every30Minutes:
                        canExecuteMonitoring = minutesSinceLastJob >= 30;
                        break;
                    case SqlSenseiRunMonitoringPeriod.Every60Minutes:
                        canExecuteMonitoring = minutesSinceLastJob >= 60;
                        break;
                }
            }

            var maintenanceJob = new JobExecution();
            var monitoringJob = new JobExecution();

            if (canExecuteMaintenance)
            {
                maintenanceJob = new JobExecution()
                {
                    CompanyFk = companyResult.Value.Id,
                    ServerFk = companyResult.Value.CurrentServer.Id,
                    Type = JobType.Maintenance,
                    Status = JobStatus.InProgress,
                    CreatedOn = DateTime.UtcNow,
                };

                DbContext.Jobs.Add(maintenanceJob);
            }

            if (canExecuteMonitoring)
            {
                monitoringJob = new JobExecution()
                {
                    CompanyFk = companyResult.Value.Id,
                    ServerFk = companyResult.Value.CurrentServer.Id,
                    Type = JobType.Maintenance,
                    Status = JobStatus.InProgress,
                    CreatedOn = DateTime.UtcNow,
                };

                DbContext.Jobs.Add(monitoringJob);
            }

            await DbContext.SaveChangesAsync();

            return Result.Ok(new CanExecuteJobsResponse(
                canExecuteMaintenance,
                canExecuteMonitoring,
                canExecuteMonitoring,
                maintenanceJob.Id,
                monitoringJob.Id));
        }
    }
}
