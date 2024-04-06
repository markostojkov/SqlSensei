using SqlSensei.Core;

using System.Collections.Generic;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfiguration : ISqlSenseiConfiguration
    {
        // interface props
        public SqlSenseiConfigurationOptions Configuration => SqlSenseiConfigurationOptions.SqlServer;
        public List<SqlSenseiConfigurationDatabase> Databases { get; }
        public string MonitoringAndMaintenanceScriptDatabaseConnection { get; }
        public string ApiKey { get; }
        public bool ReportErrorsToSqlSensei { get; }
        public string ApiVersion { get; } = SqlSenseiConfigurationApiVersion.Version1;
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
            Databases = databases;
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