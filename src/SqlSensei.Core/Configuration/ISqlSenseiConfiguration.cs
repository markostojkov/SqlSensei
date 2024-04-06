using System.Collections.Generic;

namespace SqlSensei.Core
{
    public interface ISqlSenseiConfiguration
    {
        SqlSenseiConfigurationOptions Configuration { get; }
        List<SqlSenseiConfigurationDatabase> Databases { get; }
        string MonitoringAndMaintenanceScriptDatabaseConnection { get; }
        string ApiKey { get; }
        bool ReportErrorsToSqlSensei { get; }
        string ApiVersion { get; }
    }
}