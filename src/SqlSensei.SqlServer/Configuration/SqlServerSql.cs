namespace SqlSensei.SqlServer
{
    public static class SqlServerSql
    {
        #region Monitoring

        public static string MonitoringMissingIndexTableLogTo => "MissingIndexMonitoring";
        public static string MonitoringUsageIndexTableLogTo => "UsageIndexMonitoring";
        private static string MonitoringMissingIndexTableName => "[dbo].[MissingIndexMonitoring]";
        private static string MonitoringUsageIndexTableName => "[dbo].[UsageIndexMonitoring]";

        public static string MonitoringMissingIndexTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringMissingIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringMissingIndexTableName}";

        public static string MonitoringUsageIndexTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringUsageIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringUsageIndexTableName}";

        public static string MonitoringMissingIndexSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringMissingIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringMissingIndexTableName}";

        public static string MonitoringUsageIndexSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringUsageIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringUsageIndexTableName}";

        #endregion

        #region Maintenance

        public static string MaintenanceResultsTable => "[dbo].[CommandLog]";

        public static string MaintenanceTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'CommandLog' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MaintenanceResultsTable}";

        public static string MaintenanceSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'CommandLog' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MaintenanceResultsTable}";

        #endregion
    }
}