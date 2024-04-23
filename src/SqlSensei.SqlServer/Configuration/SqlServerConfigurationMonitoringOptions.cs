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

            EXECUTE dbo.sp_BlitzCache
            @SortOrder = '{1}',
            @OutputDatabaseName = '{0}' ,
            @OutputSchemaName = 'dbo',
            @OutputTableName = '" + SqlServerSql.MonitoringQueryTableLogTo + @"'

            GO
        ";

        public string ScriptName { get; }

        private SqlServerConfigurationMonitoringOptions(string scriptName)
        {
            ScriptName = scriptName;
        }

        public static SqlServerConfigurationMonitoringOptions Default => new("MonitoringSolution.sql");

        public string GetScript(string monitoringAndMaintenanceScriptDatabaseName, string sortQueriesBy)
        {
            var scriptForAllDatabases = string.Format(_scriptMonitoringWholeServer, monitoringAndMaintenanceScriptDatabaseName, sortQueriesBy);

            scriptForAllDatabases += string.Format(_script, monitoringAndMaintenanceScriptDatabaseName);

            return scriptForAllDatabases;
        }

        public string GetCurrentServerStateScript()
        {
            return @"EXECUTE dbo.sp_BlitzFirst  @ExpertMode = 1, @Seconds = 30, @OutputResultSets = N'Findings|WaitStats'";
        }
    }
}