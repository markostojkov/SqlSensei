#nullable enable

using System.Collections.Generic;
using System.Linq;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfiguration : ISqlSenseiConfiguration
    {
        private string _maintenanceScriptExecution { get; }
        private string _monitoringIndexLogScriptExecution { get; }

        public SqlSenseiConfigurationOptions Configuration => SqlSenseiConfigurationOptions.SqlServer;
        public List<string> MonitoringScripts { get; }
        public List<string> MaintenanceScripts { get; }
        public string ConnectionString { get; }
        public List<string> Databases { get; }
        public int MaintenanceScriptDropLogsOlderThanDays { get; }

        private SqlServerConfiguration(
            string connectionString,
            List<string> databases,
            int maintenanceScriptDropLogsOlderThanDays,
            SqlServerConfigurationMonitoringOptions? monitoringOptions,
            SqlServerConfigurationMaintenanceOptions? maintenanceOptions)
        {
            MonitoringScripts = new List<string>();
            MaintenanceScripts = new List<string>();
            ConnectionString = connectionString;
            Databases = databases;
            MaintenanceScriptDropLogsOlderThanDays = maintenanceScriptDropLogsOlderThanDays;
            _maintenanceScriptExecution = string.Empty;
            _monitoringIndexLogScriptExecution = string.Empty;

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            if (monitoringOptions is not null)
            {
                MonitoringScripts = new() { monitoringOptions.ScriptName };
                _monitoringIndexLogScriptExecution = monitoringOptions.GetIndexLoggingScript(connectionStringBuilder.InitialCatalog, databases);
            }

            if (maintenanceOptions is not null)
            {
                MaintenanceScripts = new() { maintenanceOptions.ScriptName };
                _maintenanceScriptExecution = maintenanceOptions.GetScript(databases);
            }
        }

        public static SqlServerConfiguration Default(string connectionString, params string[] databasesToPerformActionsOn)
        {
            return new SqlServerConfiguration(connectionString,
                databasesToPerformActionsOn.ToList(),
                30,
                SqlServerConfigurationMonitoringOptions.CoreNoQueryStore,
                SqlServerConfigurationMaintenanceOptions.OlaHallengrenDefault);
        }

        public static SqlServerConfiguration Create(string connectionString,
            int keepLogForDays = 30,
            SqlServerConfigurationMonitoringOptions? monitoringOptions = null,
            SqlServerConfigurationMaintenanceOptions? maintenanceOptions = null,
            params string[] databasesToPerformActionsOn)
        {
            return new SqlServerConfiguration(connectionString,
                databasesToPerformActionsOn.ToList(),
                keepLogForDays,
                monitoringOptions,
                maintenanceOptions);
        }

        public string GetMaintenanceScript()
        {
            return _maintenanceScriptExecution;
        }

        public string GetMonitoringLog()
        {
            return _monitoringIndexLogScriptExecution;
        }
    }
}