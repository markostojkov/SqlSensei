using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface IServiceLogger
    {
        Task<Result<CanExecuteJobsResponse>> GetCanExecuteJobs();
        Task LogMaintenance(long jobId, IEnumerable<IMaintenanceJobLog> logs);
        Task LogMonitoring(
            long jobId,
            IEnumerable<IMonitoringJobIndexMissingLog> logsMissingIndex,
            IEnumerable<IMonitoringJobIndexUsageLog> logsUsageIndex,
            IEnumerable<IMonitoringJobServerLog> logsServer,
            IEnumerable<IMonitoringJobServerPerformanceLogWaitStat> logsWaitStatsServer,
            IEnumerable<IMonitoringJobServerPerformanceLogFinding> logsFindingsServer);
    }
}