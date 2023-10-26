using System.Collections.Generic;

namespace SqlSensei.Core
{
    public interface ISqlSenseiConfiguration
    {
        SqlSenseiConfigurationOptions Configuration { get; }
        List<string> MonitoringScripts { get; }
        List<string> MaintenanceScripts { get; }
        string ConnectionString { get; }
        List<string> Databases { get; }
        int DropLogsOlderThanDays { get; }
        string GetMaintenanceScript();
        string GetMonitoringLog();
    }
}
