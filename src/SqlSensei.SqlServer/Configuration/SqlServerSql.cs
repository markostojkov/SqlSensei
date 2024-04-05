namespace SqlSensei.SqlServer
{
    public static class SqlServerSql
    {
        #region Monitoring

        public static string MonitoringMissingIndexTableLogTo => "MissingIndexMonitoring";
        public static string MonitoringUsageIndexTableLogTo => "UsageIndexMonitoring";
        private static string MonitoringMissingIndexTableName => "[dbo].[MissingIndexMonitoring]";
        private static string MonitoringUsageIndexTableName => "[dbo].[UsageIndexMonitoring]";
        public static string MonitoringMissingIndexTruncateLogTable => $"TRUNCATE TABLE {MonitoringMissingIndexTableName}";
        public static string MonitoringUsageIndexTruncateLogTable => $"TRUNCATE TABLE {MonitoringUsageIndexTableName}";
        public static string MonitoringMissingIndexSelectLogTable => $"SELECT * FROM {MonitoringMissingIndexTableName}";
        public static string MonitoringUsageIndexSelectLogTable => $"SELECT * FROM {MonitoringUsageIndexTableName}";
        #endregion

        #region Maintenance

        public static string MaintenanceResultsTable => "[dbo].[CommandLog]";
        public static string MaintenanceTruncateLogTable => $"TRUNCATE TABLE {MaintenanceResultsTable}";
        public static string MaintenanceSelectLogTable => $"SELECT * FROM {MaintenanceResultsTable}";

        #endregion
    }
}
