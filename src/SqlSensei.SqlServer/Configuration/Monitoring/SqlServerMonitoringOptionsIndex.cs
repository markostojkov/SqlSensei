namespace SqlSensei.SqlServer
{
    public class SqlServerMonitoringOptionsIndex
    {
        private static readonly string _replaceDatabase = "replace-database";
        private readonly string _scriptExecution;
        private readonly string _script = @"
            EXECUTE dbo.sp_BlitzIndex
            @DatabaseName       = '" + _replaceDatabase + @"',
            @Mode               = 3,
            @OutputDatabaseName = '{0}',
            @OutputSchemaName   = 'dbo',
            @OutputTableName    = 'IndexMonitoring'
        ";

        private SqlServerMonitoringOptionsIndex(string logToDatabaseName)
        {
            _scriptExecution = string.Format(_script, logToDatabaseName);
        }

        public static SqlServerMonitoringOptionsIndex Create(string outputDatabase)
        {
            return new SqlServerMonitoringOptionsIndex(outputDatabase);
        }

        public string GetScript(string[] databases)
        {
            string scriptForAllDatabases = string.Empty;

            foreach (string databaseName in databases)
            {
                scriptForAllDatabases += _scriptExecution.Replace(_replaceDatabase, databaseName);
            }

            return scriptForAllDatabases;
        }

        public string GetScript(string database)
        {
            return _scriptExecution.Replace(_replaceDatabase, database);
        }
    }
}
