using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.SqlServer
{
    public class SqlServerConfigurationMonitoringOptions
    {
        private SqlServerConfigurationMonitoringOptions(string scriptName)
        {
            ScriptName = scriptName;
        }

        public static SqlServerConfigurationMonitoringOptions CoreNoQueryStore => new("Install-Core-Blitz-No-Query-Store.sql");

        public string ScriptName { get; }

        public string GetIndexLoggingScript(string logToDatabase, IEnumerable<string> databases)
        {
            return SqlServerMonitoringOptionsIndex.Create(logToDatabase).GetScript(databases.ToArray());
        }

        public string GetIndexLoggingScript(string logToDatabase, string[] databases)
        {
            return SqlServerMonitoringOptionsIndex.Create(logToDatabase).GetScript(databases);
        }
    }
}
