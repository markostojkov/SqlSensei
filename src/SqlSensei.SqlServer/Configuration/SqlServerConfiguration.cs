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
        public string DashboardPath { get; }
        // interface props

        public SqlServerConfigurationMonitoringOptions MonitoringOptions { get; }
        public SqlServerConfigurationMaintenanceOptions MaintenanceOptions { get; }

        private SqlServerConfiguration(
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            string dashboardPath,
            SqlServerConfigurationMonitoringOptions monitoringOptions,
            SqlServerConfigurationMaintenanceOptions maintenanceOptions)
        {
            Databases = databases;
            MonitoringAndMaintenanceScriptDatabaseConnection = monitoringAndMaintenanceScriptDatabaseConnection;
            DashboardPath = dashboardPath;
            MonitoringOptions = monitoringOptions;
            MaintenanceOptions = maintenanceOptions;
        }

        public static SqlServerConfiguration Default(
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            string dashboardPath)
        {
            return new SqlServerConfiguration(databases,
                monitoringAndMaintenanceScriptDatabaseConnection,
                dashboardPath,
                SqlServerConfigurationMonitoringOptions.Default,
                SqlServerConfigurationMaintenanceOptions.Default);
        }

        public static SqlServerConfiguration Create(
            List<SqlSenseiConfigurationDatabase> databases,
            string monitoringAndMaintenanceScriptDatabaseConnection,
            string dashboardPath,
            SqlServerConfigurationMonitoringOptions monitoringOptions,
            SqlServerConfigurationMaintenanceOptions maintenanceOptions)
        {
            return new SqlServerConfiguration(databases,
                monitoringAndMaintenanceScriptDatabaseConnection,
                dashboardPath,
                monitoringOptions,
                maintenanceOptions);
        }
    }
}