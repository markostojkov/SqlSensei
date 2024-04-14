using SqlSensei.Core;

using System.Collections.Generic;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfigurationMonitoringOptions
    {
        private static readonly string _replaceDatabase = "replace-database";
        private readonly string _script = @"
            EXECUTE dbo.sp_BlitzIndex
            @DatabaseName       = '" + _replaceDatabase + @"',
            @Mode               = 3,
            @OutputDatabaseName = '{0}',
            @OutputSchemaName   = 'dbo',
            @OutputTableName    = '" + SqlServerSql.MonitoringMissingIndexTableLogTo + @"'
    
            GO            

            EXECUTE dbo.sp_BlitzIndex
            @DatabaseName       = '" + _replaceDatabase + @"',
            @Mode               = 2,
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
        ";

        public string ScriptName { get; }

        private SqlServerConfigurationMonitoringOptions(string scriptName)
        {
            ScriptName = scriptName;
        }

        public static SqlServerConfigurationMonitoringOptions Default => new("MonitoringSolution.sql");

        public string GetScript(IEnumerable<SqlSenseiConfigurationDatabase> databases, string monitoringAndMaintenanceScriptDatabaseName)
        {
            var scriptForAllDatabases = string.Format(_scriptMonitoringWholeServer, monitoringAndMaintenanceScriptDatabaseName, monitoringAndMaintenanceScriptDatabaseName);

            foreach (var database in databases)
            {
                scriptForAllDatabases += string.Format(_script, monitoringAndMaintenanceScriptDatabaseName).Replace(_replaceDatabase, database.Database);
            }

            return scriptForAllDatabases;
        }
    }
}