using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.Insights
{
    public class SqlServerInsights(SqlSenseiDbContext dbContext)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;

        public async Task<Result<InsightsResponse>> GetInisights(long jobId)
        {
            var job = await DbContext.Jobs.FirstOrDefaultAsync(x => x.Id == jobId);

            if (job == null)
            {
                return Result.NotFound<InsightsResponse>(ResultCodes.JobNotFound);
            }

            var serverIssues = await DbContext.MonitoringJobServerLogs
                .Where(x => x.JobFk ==  jobId)
                .ToListAsync();

            var serverPerformanceIssues = await DbContext.MonitoringJobServerFindingLogs
                .Where(x => x.JobFk == jobId)
                .ToListAsync();

            var serverWaitStats = await DbContext.MonitoringJobServerWaitStatLogs
                .Where(x => x.JobFk == jobId)
                .ToListAsync();

            var queries = await DbContext.MonitoringQueryLogs
                .Where(x => x.JobFk == jobId)
                .ToListAsync();

            var indexUsage = await DbContext.MonitoringJobIndexUsageLogs
                .Where(x => x.JobFk == jobId)
                .ToListAsync();

            var missingIndex = await DbContext.MonitoringJobIndexMissingLogs
                .Where(x => x.JobFk == jobId)
                .ToListAsync();

            return Result.Ok(new InsightsResponse(
                SqlServerServerCheckIssues.GetSqlServerChecks(serverIssues),
                SqlServerServerPerformanceCheckIssues.GetSqlServerPerformanceFindings(serverPerformanceIssues, serverWaitStats, queries),
                SqlServerIndexIssues.GetSqlServerChecks(indexUsage, missingIndex)));
        }
    }
}
