using SqlSensei.Core;

using System.Collections.Generic;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfiguration : ISqlSenseiConfiguration
    {
        // interface props
        public SqlSenseiConfigurationOptions Configuration => SqlSenseiConfigurationOptions.SqlServer;
        public SqlSenseiConfigurationApiVersion ApiVersion { get; } = SqlSenseiConfigurationApiVersion.Version1;
        public List<SqlSenseiConfigurationDatabase> DatabasesForMaintenance { get; }
        public string MonitoringAndMaintenanceScriptDatabaseConnection { get; }
        public string ApiKey { get; }
        public bool ReportErrorsToSqlSensei { get; }
        // interface props

        public SqlServerConfigurationMonitoringOptions MonitoringOptions { get; }
        public SqlServerConfigurationMaintenanceOptions MaintenanceOptions { get; }

        private SqlServerConfiguration(
            string apiKey,
            bool reportErrorsToSqlSensei,
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            SqlServerConfigurationMonitoringOptions monitoringOptions,
            SqlServerConfigurationMaintenanceOptions maintenanceOptions)
        {
            ApiKey = apiKey;
            ReportErrorsToSqlSensei = reportErrorsToSqlSensei;
            DatabasesForMaintenance = databases;
            MonitoringAndMaintenanceScriptDatabaseConnection = monitoringAndMaintenanceScriptDatabaseConnection;
            MonitoringOptions = monitoringOptions;
            MaintenanceOptions = maintenanceOptions;
        }

        public static SqlServerConfiguration Default(
            string apiKey,
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            bool reportErrorsToSqlSensei = true)
        {
            return new SqlServerConfiguration(
                apiKey,
                reportErrorsToSqlSensei,
                databases,
                monitoringAndMaintenanceScriptDatabaseConnection,
                SqlServerConfigurationMonitoringOptions.Default,
                SqlServerConfigurationMaintenanceOptions.Default);
        }

        public static SqlServerConfiguration Create(
            string apiKey,
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            SqlServerConfigurationMonitoringOptions monitoringOptions,
            SqlServerConfigurationMaintenanceOptions maintenanceOptions,
            bool reportErrorsToSqlSensei = true)
        {
            return new SqlServerConfiguration(
                apiKey,
                reportErrorsToSqlSensei,
                databases,
                monitoringAndMaintenanceScriptDatabaseConnection,
                monitoringOptions,
                maintenanceOptions);
        }
    }
}