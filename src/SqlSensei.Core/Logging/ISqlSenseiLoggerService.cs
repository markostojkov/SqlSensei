using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface ISqlSenseiLoggerService
    {
        Task MaintenanceInformation(IEnumerable<IMaintenanceJobLog> jobLogs, string database);
        Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> indexLogs, IEnumerable<IMonitoringJobIndexLogUsage> indexLogsUsage, string database);
        Task Error(Exception exception, string message);
        Task Error(string message);
    }
}
