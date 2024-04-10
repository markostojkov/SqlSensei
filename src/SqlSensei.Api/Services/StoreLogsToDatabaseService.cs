using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.Services
{
    public class StoreLogsToDatabaseService(SqlSenseiDbContext dbContext, CurrentCompanyService company)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;
        public CurrentCompanyService Company { get; } = company;

        public async Task<Result> StoreMaintenanceLogs(long jobId, MaintenanceLogRequest logs)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return companyResult;
            }

            var job = await DbContext.Jobs
                .FirstOrDefaultAsync(x => x.Id == jobId && x.CompanyFk == companyResult.Value.Id && x.Status == JobStatus.InProgress);

            if (job == null)
            {
                return Result.NotFound(ResultCodes.JobNotFound);
            }

            job.Status = JobStatus.Completed;
            job.CompletedOn = DateTime.UtcNow;

            var dbLogs = logs.Logs.Select(x => new Storage.MaintenanceLog(
                companyResult.Value.Id,
                jobId,
                x.DatabaseName,
                x.Index,
                x.Statistic,
                x.IsError,
                x.ErrorMessage));

            DbContext.MaintenanceLogs.AddRange(dbLogs);

            _ = await DbContext.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result> StoreMonitoringLogs(long jobId, MonitoringLogRequest logs)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return companyResult;
            }

            var job = await DbContext.Jobs
                .FirstOrDefaultAsync(x => x.Id == jobId && x.CompanyFk == companyResult.Value.Id && x.Status == JobStatus.InProgress);

            if (job == null)
            {
                return Result.NotFound(ResultCodes.JobNotFound);
            }

            job.Status = JobStatus.Completed;
            job.CompletedOn = DateTime.UtcNow;

            var dbLogsUsageIndex = logs.IndexUsageLogs.Select(x => new Storage.MonitoringJobIndexUsageLog(
                companyResult.Value.Id,
                jobId,
                x.DatabaseName,
                x.IsClusteredIndex,
                x.IndexName,
                x.TableName,
                x.IndexDetails,
                x.Usage,
                x.ReadsUsage,
                x.WriteUsage));

            DbContext.MonitoringJobIndexUsageLogs.AddRange(dbLogsUsageIndex);

            var dbLogsMissingIndex = logs.IndexMissingLogs.Select(x => new Storage.MonitoringJobIndexMissingLog(
                companyResult.Value.Id,
                jobId,
                x.DatabaseName,
                x.TableName,
                x.MagicBenefitNumber,
                x.Impact,
                x.IndexDetails));

            DbContext.MonitoringJobIndexMissingLogs.AddRange(dbLogsMissingIndex);

            _ = await DbContext.SaveChangesAsync();

            return Result.Ok();
        }
    }
}
