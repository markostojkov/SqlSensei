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

        public string ScriptName { get; }

        private SqlServerConfigurationMonitoringOptions(string scriptName)
        {
            ScriptName = scriptName;
        }

        public static SqlServerConfigurationMonitoringOptions Default => new("MonitoringSolution.sql");

        public string GetScript(IEnumerable<SqlSenseiConfigurationDatabase> databases, string monitoringAndMaintenanceScriptDatabaseName)
        {
            var scriptForAllDatabases = string.Empty;

            foreach (var database in databases)
            {
                scriptForAllDatabases += string.Format(_script, monitoringAndMaintenanceScriptDatabaseName).Replace(_replaceDatabase, database.Database);
            }

            return scriptForAllDatabases;
        }
    }
}
