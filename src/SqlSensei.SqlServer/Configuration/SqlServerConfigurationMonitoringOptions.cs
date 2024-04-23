namespace SqlSensei.SqlServer
{
    public class SqlServerConfigurationMonitoringOptions
    {
        private readonly string _script = @"
            EXECUTE dbo.sp_BlitzIndex
            @Mode               = 3,
            @GetAllDatabases    = 1,
            @OutputDatabaseName = '{0}',
            @OutputSchemaName   = 'dbo',
            @OutputTableName    = '" + SqlServerSql.MonitoringMissingIndexTableLogTo + @"'
    
            GO            

            EXECUTE dbo.sp_BlitzIndex
            @Mode               = 2,
            @GetAllDatabases    = 1,
            @OutputDatabaseName = '{0}',
            @OutputSchemaName   = 'dbo',
            @OutputTableName    = '" + SqlServerSql.MonitoringUsageIndexTableLogTo + @"'
        ";
        private readonly string _scriptMonitoringWholeServer = @"
            EXECUTE dbo.sp_Blitz 
            @CheckServerInfo = 1,
            @OutputDatabaseName = '{0}',
            @OutputSchemaName   = 'dbo',
            @OutputTableName    = '" + SqlServerSql.MonitoringServerTableLogTo + @"'
            
            GO

            EXECUTE dbo.sp_BlitzFirst
            @ExpertMode = 1,
            @Seconds = 30,
            @OutputResultSets = N'Findings|WaitStats',
            @OutputDatabaseName = '{1}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringServerFindingsTableLogTo + @"',
            @OutputTableNameWaitStats = '" + SqlServerSql.MonitoringServerWaitStatsTableLogTo + @"'

            GO

            EXECUTE dbo.sp_BlitzCache
            @ExpertMode = 1,
            @OutputDatabaseName = '{2}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryCpuTableLogTo + @"'

            GO

            EXECUTE dbo.sp_BlitzCache
            @ExpertMode = 1,
            @OutputDatabaseName = '{3}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryReadsTableLogTo + @"'

            GO

            EXECUTE dbo.sp_BlitzCache
            @ExpertMode = 1,
            @OutputDatabaseName = '{4}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryWritesTableLogTo + @"'

            GO

            EXECUTE dbo.sp_BlitzCache
            @ExpertMode = 1,
            @OutputDatabaseName = '{5}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryDurationTableLogTo + @"'

            GO

            EXECUTE dbo.sp_BlitzCache
            @ExpertMode = 1,
            @OutputDatabaseName = '{6}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryMemoryGrantTableLogTo + @"'

            GO

        ";

        public string ScriptName { get; }

        private SqlServerConfigurationMonitoringOptions(string scriptName)
        {
            ScriptName = scriptName;
        }

        public static SqlServerConfigurationMonitoringOptions Default => new("MonitoringSolution.sql");

        public string GetScript(string monitoringAndMaintenanceScriptDatabaseName)
        {
            var scriptForAllDatabases = string.Format(
                _scriptMonitoringWholeServer,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName,
                monitoringAndMaintenanceScriptDatabaseName);

            scriptForAllDatabases += string.Format(_script, monitoringAndMaintenanceScriptDatabaseName);

            return scriptForAllDatabases;
        }
    }
}