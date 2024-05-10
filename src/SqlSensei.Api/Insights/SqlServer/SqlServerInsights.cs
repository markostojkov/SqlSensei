using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Storage;
using SqlSensei.Core;
using System.Collections.Generic;

namespace SqlSensei.Api.Insights
{
    public class SqlServerInsights(SqlSenseiDbContext dbContext, CurrentCompanyService company)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;
        public CurrentCompanyService Company { get; } = company;

        public async Task<Result<ServerResponse>> GetServerInfo(long serverId)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<ServerResponse>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.Where(x => x.Status == JobStatus.Completed).OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<ServerResponse>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<ServerResponse>(ResultCodes.JobNotFound);
            }

            var latestJob = await DbContext.Jobs.FirstOrDefaultAsync(x => x.Id == server.Jobs.First().Id);

            if (latestJob == null)
            {
                return Result.NotFound<ServerResponse>(ResultCodes.JobNotFound);
            }

            var serverIssues = await DbContext.MonitoringJobServerLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            return Result.Ok(new ServerResponse(
                server.Id,
                server.Name,
                server.ApiKey,
                SqlServerServerCheckIssues.GetServerInfo(serverIssues)));
        }

        public async Task<Result<QueryPlanResponse>> GetQueryPlan(long serverId, long queryId)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<QueryPlanResponse>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.Where(x => x.Status == JobStatus.Completed).OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<QueryPlanResponse>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<QueryPlanResponse>(ResultCodes.JobNotFound);
            }

            var latestJob = await DbContext.Jobs.FirstOrDefaultAsync(x => x.Id == server.Jobs.First().Id);

            if (latestJob == null)
            {
                return Result.NotFound<QueryPlanResponse>(ResultCodes.JobNotFound);
            }

            var queryPlan = await DbContext.MonitoringQueryLogs
                .FirstOrDefaultAsync(x => x.Id == queryId);

            if (queryPlan == null)
            {
                return Result.NotFound<QueryPlanResponse>(ResultCodes.QueryNotFound);
            }

            return Result.Ok(new QueryPlanResponse(queryPlan.QueryPlan, queryPlan.QueryText));
        }

        public async Task<Result<IEnumerable<MaintenanceResponse>>> GetServerMaintenance(long serverId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddSeconds(-1);

            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<IEnumerable<MaintenanceResponse>>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.Where(x => x.Status == JobStatus.Completed).OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<IEnumerable<MaintenanceResponse>>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<IEnumerable<MaintenanceResponse>>(ResultCodes.JobNotFound);
            }

            var maintenanceLogs = await DbContext.MaintenanceLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            var response = maintenanceLogs
                .Select(x => new MaintenanceResponse(x.DatabaseName, x.Index, x.Statistic, x.IsError, x.ErrorMessage));

            return Result.Ok(response);
        }

        public async Task<Result<InsightsResponse>> GetInsights(long serverId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddSeconds(-1);

            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<InsightsResponse>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.Where(x => x.Status == JobStatus.Completed).OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<InsightsResponse>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<InsightsResponse>(ResultCodes.JobNotFound);
            }

            var serverIssues = await DbContext.MonitoringJobServerLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            var serverFindings = await DbContext.MonitoringJobServerFindingLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            var waits = await DbContext.MonitoringJobServerWaitStatLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            var topWaitType = SqlServerServerPerformanceCheckIssues.GetTopWaitType(waits);

            var queries = await DbContext.MonitoringQueryLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .Where(x => x.WaitType == topWaitType)
                .Where(x => x.QueryHash != null)
                .ToListAsync();

            var indexUsage = await DbContext.MonitoringJobIndexUsageLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            var missingIndex = await DbContext.MonitoringJobIndexMissingLogs
                .Where(x => x.Job.ServerFk == serverId)
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Job.CompletedOn >= startOfDay && x.Job.CompletedOn <= endOfDay)
                .ToListAsync();

            return Result.Ok(new InsightsResponse(
                SqlServerServerCheckIssues.GetSqlServerChecks(serverIssues, serverFindings),
                SqlServerServerPerformanceCheckIssues.GetSqlServerPerformanceFindings(queries, topWaitType),
                SqlServerIndexIssues.GetSqlServerChecks(indexUsage, missingIndex)));
        }

        public async Task<Result<IEnumerable<SqlServerPerformanceWaitStatGraph>>> GetWaitStats(long serverId, DateTime start, DateTime end)
        {
            if (end < start && (end - start).TotalDays > 7)
            {
                return Result.Forbidden<IEnumerable<SqlServerPerformanceWaitStatGraph>>(ResultCodes.ServerWaitStatsCantBeMoreThan7Days);
            }

            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<IEnumerable<SqlServerPerformanceWaitStatGraph>>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<IEnumerable<SqlServerPerformanceWaitStatGraph>>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<IEnumerable<SqlServerPerformanceWaitStatGraph>>(ResultCodes.JobNotFound);
            }

            var serverWaitStatsGraph = await DbContext.MonitoringJobServerWaitStatLogs
                .Include(x => x.Job)
                .Where(x => x.Job.ServerFk == server.Id)
                .Where(x => x.Job.CreatedOn < end && x.Job.CreatedOn > start)
                .ToListAsync();

            return Result.Ok(SqlServerServerPerformanceCheckIssues.GetSqlServerWaitStatsGraph(serverWaitStatsGraph));
        }

        public async Task<Result<IEnumerable<SqlServerPerformancePerformanceGraph>>> GetPerformanceStats(long serverId, DateTime start, DateTime end)
        {
            if (end < start && (end - start).TotalDays > 7)
            {
                return Result.Forbidden<IEnumerable<SqlServerPerformancePerformanceGraph>>(ResultCodes.ServerWaitStatsCantBeMoreThan7Days);
            }

            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<IEnumerable<SqlServerPerformancePerformanceGraph>>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.OrderByDescending(y => y.Id).Take(1))
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .Where(x => x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server is null)
            {
                return Result.NotFound<IEnumerable<SqlServerPerformancePerformanceGraph>>(ResultCodes.ServerNotFound);
            }

            if (server.Jobs.Any() is false)
            {
                return Result.NotFound<IEnumerable<SqlServerPerformancePerformanceGraph>>(ResultCodes.JobNotFound);
            }

            var serverPerformanceGraph = await DbContext.MonitoringJobServerFindingLogs
                .Include(x => x.Job)
                .Where(x => x.Job.ServerFk == server.Id)
                .Where(x => x.Job.CreatedOn < end && x.Job.CreatedOn > start)
                .Where(x => SqlServerServerPerformanceCheckIssues.PerformanceCheckIds.Contains(x.CheckId))
                .ToListAsync();

            return Result.Ok(SqlServerServerPerformanceCheckIssues.GetSqlServerPerformanceGraph(serverPerformanceGraph));
        }
    }
}
