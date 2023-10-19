using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;
using SqlSensei.SqlServer.InformationGather;

namespace SqlSensei.SqlServer
{
    public class SqlServerJob : SqlServerBase, ISqlSenseiJob
    {
        public SqlServerJob(ISqlSenseiConfiguration configuration, ISqlSenseiLoggerService loggerService) : base(loggerService, configuration)
        {
            Configuration = configuration;
            LoggerService = loggerService;
        }

        public async Task ExecuteMaintenanceJob()
        {
            await ExecuteCommandAsyncNoTransaction(Configuration.GetMaintenanceScript());

            // Go through all databases in configuration, DELETE older rows, do maintenance and log
            foreach (var databaseName in Configuration.Databases)
            {
                var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.MaintenanceScriptDropLogsOlderThanDays);

                await ExecuteCommandAsync(
                    "DELETE FROM [dbo].[CommandLog] WHERE StartTime < @DeleteDate AND DatabaseName = @DatabaseName",
                    new SqlParameter("@DeleteDate", deleteRecordsOlderThan),
                    new SqlParameter("@DatabaseName", databaseName));

                var loggingInformation = new List<CommandLog>();

                var result = await ExecuteCommandAsync(
                    "SELECT * FROM [dbo].[CommandLog] WHERE DatabaseName = @DatabaseName",
                    (reader) =>
                    {
                        loggingInformation = CommandLog.GetAll(reader);
                    },
                    new SqlParameter("@DatabaseName", databaseName));

                if (!result)
                {
                    await LoggerService.Error("ExecuteMaintenanceJob logging information error");
                    return;
                }

                await LoggerService.MaintenanceInformation(loggingInformation, databaseName);
            }
        }

        public async Task ExecuteMonitoringLogJob()
        {
            await ExecuteCommandAsyncNoTransaction(Configuration.GetMonitoringLog());
        }

        public async Task ExecuteMonitoringJob()
        {
            foreach (var databaseName in Configuration.Databases)
            {
                var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.MaintenanceScriptDropLogsOlderThanDays);

                await ExecuteCommandAsync(
                    @"IF OBJECT_ID('[dbo].[IndexMonitoring]', 'U') IS NOT NULL
                      BEGIN
                            DELETE FROM [dbo].[IndexMonitoring] WHERE run_datetime < @DeleteDate AND database_name = @DatabaseName;
                      END",
                    new SqlParameter("@DeleteDate", deleteRecordsOlderThan),
                    new SqlParameter("@DatabaseName", databaseName));

                var monitoringIndexLogCommand = @"
                    IF OBJECT_ID('[dbo].[IndexMonitoring]', 'U') IS NOT NULL
                        BEGIN
                            WITH CTE AS (
                                SELECT
                                    *,
                                    ROW_NUMBER() OVER (PARTITION BY create_tsql ORDER BY ID) AS RowNum
                                FROM [dbo].[IndexMonitoring])

                            SELECT *
                            FROM CTE
                            WHERE RowNum = 1 AND database_name = @DatabaseName;
                        END
                    ";

                var loggingInformation = new List<IndexLog>();

                var result = await ExecuteCommandAsync(
                    monitoringIndexLogCommand,
                    (reader) =>
                    {
                        loggingInformation = IndexLog.GetAll(reader);
                    },
                    new SqlParameter("@DatabaseName", databaseName));

                if (!result)
                {
                    await LoggerService.Error("ExecuteMonitoringJob logging information error");
                    return;
                }

                await LoggerService.MonitoringInformation(loggingInformation, databaseName);
            }
        }

        public void InstallMaintenanceAndMonitoringScripts()
        {
            Task.Run(async () =>
            {
                foreach (var scriptName in Configuration.MaintenanceScripts)
                {
                    var assemblyScriptName = typeof(SqlServerJob).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(scriptName));

                    using var assemblyScriptStream = typeof(SqlServerJob).Assembly.GetManifestResourceStream(assemblyScriptName);
                    using var assemblyScriptStreamReader = new StreamReader(assemblyScriptStream);
                    var scriptContent = assemblyScriptStreamReader.ReadToEnd();

                    await ExecuteScriptAsyncGoStatements(scriptContent);
                }

                foreach (var scriptName in Configuration.MonitoringScripts)
                {
                    var assemblyScriptName = typeof(SqlServerJob).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(scriptName));

                    using var assemblyScriptStream = typeof(SqlServerJob).Assembly.GetManifestResourceStream(assemblyScriptName);
                    using var assemblyScriptStreamReader = new StreamReader(assemblyScriptStream);
                    var scriptContent = assemblyScriptStreamReader.ReadToEnd();

                    await ExecuteScriptAsyncGoStatements(scriptContent);
                }
            });
        }
    }
}
