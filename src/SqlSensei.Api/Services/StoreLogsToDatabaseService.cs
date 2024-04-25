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

            var dbLogsServer = logs.ServerLogs.Select(x => new Storage.MonitoringJobServerLog(
                companyResult.Value.Id,
                jobId,
                x.DatabaseName,
                x.Priority,
                x.CheckId,
                x.Details));

            DbContext.MonitoringJobServerLogs.AddRange(dbLogsServer);

            var dbLogsWaitStatsServer = logs.ServerWaitStatLogs.Select(x => new Storage.MonitoringJobServerWaitStatLog(
                companyResult.Value.Id,
                jobId,
                x.Type,
                x.TimeInMs));

            DbContext.MonitoringJobServerWaitStatLogs.AddRange(dbLogsWaitStatsServer);

            var dbLogsFindingsServer = logs.ServerFindingLogs.Select(x => new Storage.MonitoringJobServerFindingLog(
                companyResult.Value.Id,
                jobId,
                x.CheckId,
                x.Priority,
                x.Details));

            DbContext.MonitoringJobServerFindingLogs.AddRange(dbLogsFindingsServer);

            StoreQueryLogs(companyResult.Value.Id, jobId, logs.QueryLogs);

            _ = await DbContext.SaveChangesAsync();

            return Result.Ok();
        }

        private void StoreQueryLogs(long companyId, long jobId, IEnumerable<MonitoringJobQueryLog> logs)
        {
            var dbLogs = logs.Select(x => new MonitoringQueryLog(
                companyId,
                jobId,
                x.WaitType,
                x.TopNo,
                x.DatabaseName,
                x.QueryPlanCost,
                x.QueryText,
                x.Warnings,
                x.QueryPlan,
                x.MissingIndexes,
                x.ImplicitConversionInfo,
                x.ExecutionCount,
                x.ExecutionsPerMinute,
                x.TotalCPU,
                x.AverageCPU,
                x.TotalDuration,
                x.AverageDuration,
                x.TotalReads,
                x.AverageReads,
                x.TotalReturnedRows,
                x.AverageReturnedRows,
                x.MinReturnedRows,
                x.MaxReturnedRows,
                x.NumberOfPlans,
                x.NumberOfDistinctPlans,
                x.LastExecutionTime,
                x.QueryHash
            ));

            DbContext.MonitoringQueryLogs.AddRange(dbLogs);
        }
    }
}
