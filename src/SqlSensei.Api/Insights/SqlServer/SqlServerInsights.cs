using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.Insights
{
    public class SqlServerInsights(SqlSenseiDbContext dbContext, CurrentCompanyService company)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;
        public CurrentCompanyService Company { get; } = company;

        public async Task<Result<InsightsResponse>> GetInsights(long serverId)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<InsightsResponse>(companyResult);
            }

            var server = await DbContext.Servers
                .Include(x => x.Jobs.OrderByDescending(y => y.Id).Take(1))
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

            var latestJob = await DbContext.Jobs.FirstOrDefaultAsync(x => x.Id == server.Jobs.First().Id);

            if (latestJob == null)
            {
                return Result.NotFound<InsightsResponse>(ResultCodes.JobNotFound);
            }

            var serverIssues = await DbContext.MonitoringJobServerLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            var serverPerformanceIssues = await DbContext.MonitoringJobServerFindingLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            var serverWaitStats = await DbContext.MonitoringJobServerWaitStatLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            var queries = await DbContext.MonitoringQueryLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            var indexUsage = await DbContext.MonitoringJobIndexUsageLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            var missingIndex = await DbContext.MonitoringJobIndexMissingLogs
                .Where(x => x.JobFk == latestJob.Id)
                .ToListAsync();

            return Result.Ok(new InsightsResponse(
                server.Id,
                server.Name,
                SqlServerServerCheckIssues.GetSqlServerChecks(serverIssues),
                SqlServerServerPerformanceCheckIssues.GetSqlServerPerformanceFindings(serverPerformanceIssues, serverWaitStats, queries),
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
    }
}
