using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface IServiceLogger
    {
        Task LogMaintenance(IEnumerable<IMaintenanceJobLog> logs);
        Task LogMonitoring(IEnumerable<IMonitoringJobIndexMissingLog> logsMissingIndex, IEnumerable<IMonitoringJobIndexUsageLog> logsUsageIndex);
    }
}
