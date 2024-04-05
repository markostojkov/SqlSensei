using SqlSensei.Core;
using SqlSensei.SqlServer.InformationGather;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSensei.SqlServer
{
    public class SqlServerJob(
        SqlServerConfiguration configuration,
        ISqlSenseiErrorLoggerService errorLoggerService,
        IServiceLogger serviceLogger) : SqlServerBase(errorLoggerService, configuration), ISqlSenseiJob
    {
        public IServiceLogger ServiceLogger { get; } = serviceLogger;

        public async Task ExecuteMaintenanceJob()
        {
            var result = await ExecuteCommandAsync(SqlServerSql.MaintenanceTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsyncNoTransaction(Configuration.MaintenanceOptions.GetScript(Configuration.Databases));

            if (!result)
            {
                return;
            }

            var maintenanceResults = new List<CommandLog>();

            result = await ExecuteCommandAsync(SqlServerSql.MaintenanceSelectLogTable, (reader) => maintenanceResults = CommandLog.GetAll(reader));

            if (!result)
            {
                return;
            }

            await ServiceLogger.LogMaintenance(maintenanceResults);
        }

        public async Task ExecuteMonitoringJob()
        {
            var result = await ExecuteCommandAsync(SqlServerSql.MonitoringMissingIndexTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringUsageIndexTruncateLogTable);

            if (!result)
            {
                return;
            }

            await ExecuteScriptAsyncGoStatements(
                Configuration.MonitoringOptions.GetScript(Configuration.Databases, ConnectionStringParsed.InitialCatalog));

            var indexUsageResults = new List<IndexLogUsage>();
            var indexMissingResults = new List<IndexLog>();

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringUsageIndexSelectLogTable, (reader) => indexUsageResults = IndexLogUsage.GetAll(reader));

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringMissingIndexSelectLogTable, (reader) => indexMissingResults = IndexLog.GetAll(reader));

            if (!result)
            {
                return;
            }

            await ServiceLogger.LogMonitoring(indexMissingResults, indexUsageResults);
        }

        public void InstallMaintenanceAndMonitoringScripts()
        {
            _ = Task.Run(async () =>
            {
                await ExecuteScript(Configuration.MaintenanceOptions.ScriptName);
                await ExecuteScript(Configuration.MonitoringOptions.ScriptName);
            });
        }

        private async Task ExecuteScript(string scriptName)
        {
            var assemblyScriptName = typeof(SqlServerJob).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(scriptName));
            using var assemblyScriptStream = typeof(SqlServerJob).Assembly.GetManifestResourceStream(assemblyScriptName);
            using var assemblyScriptStreamReader = new StreamReader(assemblyScriptStream);
            var scriptContent = assemblyScriptStreamReader.ReadToEnd();

            await ExecuteScriptAsyncGoStatements(scriptContent);
        }
    }
}
