namespace SqlSensei.SqlServer
{
    public static class SqlServerSql
    {
        #region Monitoring

        public static string MonitoringQueryCpuTableLogTo => "MonitoringQueryCpuTableLogTo";
        public static string MonitoringQueryReadsTableLogTo => "MonitoringQueryReadsTableLogTo";
        public static string MonitoringQueryWritesTableLogTo => "MonitoringQueryWritesTableLogTo";
        public static string MonitoringQueryDurationTableLogTo => "MonitoringQueryDurationTableLogTo";
        public static string MonitoringQueryMemoryGrantTableLogTo => "MonitoringQueryMemoryGrantTableLogTo";
        public static string MonitoringMissingIndexTableLogTo => "MissingIndexMonitoring";
        public static string MonitoringUsageIndexTableLogTo => "UsageIndexMonitoring";
        public static string MonitoringServerTableLogTo => "ServerMonitoring";
        public static string MonitoringServerWaitStatsTableLogTo => "ServerMonitoringWaitStats";
        public static string MonitoringServerWaitStatsCategoriesTableLogTo => "ServerMonitoringWaitStats_Categories";
        public static string MonitoringServerFindingsTableLogTo => "ServerMonitoringFindings";
        private static string MonitoringMissingIndexTableName => "[dbo].[MissingIndexMonitoring]";
        private static string MonitoringUsageIndexTableName => "[dbo].[UsageIndexMonitoring]";

        public static string MonitoringQueryCpuTruncateLogTable =>
    $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryCpuTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringQueryCpuTableLogTo}";

        public static string MonitoringQueryReadsTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryReadsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringQueryReadsTableLogTo}";

        public static string MonitoringQueryWritesTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryWritesTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringQueryWritesTableLogTo}";

        public static string MonitoringQueryDurationTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryDurationTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringQueryDurationTableLogTo}";

        public static string MonitoringQueryMemoryGrantTruncateLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryMemoryGrantTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    TRUNCATE TABLE {MonitoringQueryMemoryGrantTableLogTo}";

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

        public static string MonitoringQueryCpuSelectLogTable =>
    $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryCpuTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringQueryCpuTableLogTo}";

        public static string MonitoringQueryReadsSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryReadsTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringQueryReadsTableLogTo}";

        public static string MonitoringQueryWritesSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryWritesTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringQueryWritesTableLogTo}";

        public static string MonitoringQueryDurationSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryDurationTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringQueryDurationTableLogTo}";

        public static string MonitoringQueryMemoryGrantSelectLogTable =>
            $@"IF EXISTS(SELECT * FROM sys.tables WHERE name = N'{MonitoringQueryMemoryGrantTableLogTo}' AND schema_id = SCHEMA_ID('dbo'))
    SELECT * FROM {MonitoringQueryMemoryGrantTableLogTo}";

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