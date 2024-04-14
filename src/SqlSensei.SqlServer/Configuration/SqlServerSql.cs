namespace SqlSensei.SqlServer
{
    public static class SqlServerSql
    {
        #region Monitoring

        public static string MonitoringMissingIndexTableLogTo => "MissingIndexMonitoring";
        public static string MonitoringUsageIndexTableLogTo => "UsageIndexMonitoring";
        public static string MonitoringServerTableLogTo => "ServerMonitoring";
        public static string MonitoringServerWaitStatsTableLogTo => "ServerMonitoringWaitStats";
        public static string MonitoringServerWaitStatsCategoriesTableLogTo => "ServerMonitoringWaitStats_Categories";
        public static string MonitoringServerFindingsTableLogTo => "ServerMonitoringFindings";
        private static string MonitoringMissingIndexTableName => "[dbo].[MissingIndexMonitoring]";
        private static string MonitoringUsageIndexTableName => "[dbo].[UsageIndexMonitoring]";

        public static string MonitoringMissingIndexTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringMissingIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringMissingIndexTableName}";

        public static string MonitoringUsageIndexTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringUsageIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringUsageIndexTableName}";

        public static string MonitoringServerTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringServerTableLogTo}";

        public static string MonitoringServerWaitStatsTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerWaitStatsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringServerWaitStatsTableLogTo}";

        public static string MonitoringServerWaitStatsCategoriesTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerWaitStatsCategoriesTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringServerWaitStatsCategoriesTableLogTo}";

        public static string MonitoringServerFindingsTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerFindingsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringServerFindingsTableLogTo}";

        public static string MonitoringMissingIndexSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringMissingIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringMissingIndexTableName}";

        public static string MonitoringUsageIndexSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringUsageIndexTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringUsageIndexTableName}";

        public static string MonitoringServerSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringServerTableLogTo}";

        public static string MonitoringServerWaitStatsSelectLogTable =>
           $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerWaitStatsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringServerWaitStatsTableLogTo}";

        public static string MonitoringServerWaitStatsCategoriesSelectLogTable =>
           $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerWaitStatsCategoriesTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringServerWaitStatsCategoriesTableLogTo}";

        public static string MonitoringServerFindingsSelectLogTable =>
           $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringServerFindingsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringServerFindingsTableLogTo}";

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