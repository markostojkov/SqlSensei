using SqlSensei.Core;
using SqlSensei.SqlServer.InformationGather;

using System.Collections.Generic;
using System.Data;
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

        public void StartService()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(EnvHelpers.DelayForJob());

                        var canExecuteJobsResponse = await ServiceLogger.GetCanExecuteJobs();

                        if (canExecuteJobsResponse.IsFailure)
                        {
                            continue;
                        }

                        if (canExecuteJobsResponse.Value.CanExecuteMaintenance)
                        {
                            await ExecuteMaintenanceJob(canExecuteJobsResponse.Value.MaintenanceJobId);
                        }

                        if (canExecuteJobsResponse.Value.CanExecuteMonitoring)
                        {
                            await ExecuteMonitoringJob(canExecuteJobsResponse.Value.MonitoringJobId);
                        }
                    }
                    catch (System.Exception e)
                    {
                        ErrorLoggerService.Error(e, e.Message);
                    }
                }
            });
        }

        public async Task ExecuteMaintenanceJob(long jobId)
        {
            var result = await ExecuteCommandAsync(SqlServerSql.MaintenanceTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsyncNoTransaction(Configuration.MaintenanceOptions.GetScript(Configuration.DatabasesForMaintenance));

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

            await ServiceLogger.LogMaintenance(jobId, maintenanceResults);
        }

        public async Task ExecuteMonitoringJob(long jobId)
        {
            var result = await ExecuteCommandAsync(SqlServerSql.MonitoringServerTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringQueryTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringServerWaitStatsTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringServerWaitStatsCategoriesTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringServerFindingsTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringMissingIndexTruncateLogTable);

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringUsageIndexTruncateLogTable);

            if (!result)
            {
                return;
            }

            var serverWaitStatsResults = new List<ServerWaitStatsLog>();
            var serverFindingResults = new List<ServerFindingLog>();

            result = await ExecuteCommandAsync(Configuration.MonitoringOptions.GetCurrentServerStateScript(), (reader) =>
            {
                var table1 = new DataTable();
                table1.Load(reader);

                serverFindingResults = ServerFindingLog.GetAll(table1);

                var table2 = new DataTable();
                table2.Load(reader);

                serverWaitStatsResults = ServerWaitStatsLog.GetAll(table2);
            });

            if (!result)
            {
                return;
            }

            await ExecuteScriptAsyncGoStatements(Configuration.MonitoringOptions.GetScript(ConnectionStringParsed.InitialCatalog, ServerWaitStatsLog.WaitType(serverWaitStatsResults)));

            var indexUsageResults = new List<IndexLogUsage>();
            var indexMissingResults = new List<IndexLog>();
            var serverResults = new List<ServerLog>();
            var queryResults = new List<QueryLog>();

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

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringServerSelectLogTable, (reader) => serverResults = ServerLog.GetAll(reader));

            if (!result)
            {
                return;
            }

            result = await ExecuteCommandAsync(SqlServerSql.MonitoringQuerySelectLogTable, (reader) => queryResults = QueryLog.GetAll(reader));

            if (!result)
            {
                return;
            }

            await ServiceLogger.LogMonitoring(
                jobId,
                indexMissingResults,
                indexUsageResults,
                serverResults,
                serverWaitStatsResults,
                serverFindingResults,
                queryResults);
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