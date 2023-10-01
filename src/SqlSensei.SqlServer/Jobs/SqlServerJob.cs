using System;
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
        public SqlServerJob(ISqlSenseiConfiguration configuration, ISqlSenseiLoggerService loggerService)
        {
            Configuration = configuration;
            LoggerService = loggerService;
        }

        private ISqlSenseiConfiguration Configuration { get; }
        public ISqlSenseiLoggerService LoggerService { get; }

        public async Task ExecuteMaintenanceJob()
        {
            string maintenanceLogCommand;

            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            await ExecuteScriptAsync(connection, Configuration.GetMaintenanceScript(), LoggerService);

            // Go through all databases in configuration, DELETE older rows, do maintenance and log
            foreach (var databaseName in Configuration.Databases)
            {
                var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.MaintenanceScriptDropLogsOlderThanDays);

                await ExecuteScriptAsync(
                    connection,
                    "DELETE FROM [dbo].[CommandLog] WHERE StartTime < @DeleteDate AND DatabaseName = @DatabaseName",
                    LoggerService,
                    new SqlParameter("@DeleteDate", deleteRecordsOlderThan),
                    new SqlParameter("@DatabaseName", databaseName));

                maintenanceLogCommand = "SELECT * FROM [dbo].[CommandLog] WHERE DatabaseName = @DatabaseName";

                using var reader = await ExecuteCommandAsync(connection, maintenanceLogCommand, LoggerService, new SqlParameter("@DatabaseName", databaseName));

                if (reader == null)
                {
                    await LoggerService.Error("ExecuteMaintenanceJob logging information error");

                    return;
                }

                var loggingInformation = CommandLog.GetAll(reader);

                await LoggerService.MaintenanceInformation(loggingInformation, databaseName);
            }

            connection.Close();
        }

        public async Task ExecuteMonitoringLogJob()
        {
            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            await ExecuteScriptAsync(connection, Configuration.GetMonitoringLog(), LoggerService);

            connection.Close();
        }

        public async Task ExecuteMonitoringJob()
        {
            string monitoringIndexLogCommand;

            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            foreach (var databaseName in Configuration.Databases)
            {
                var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.MaintenanceScriptDropLogsOlderThanDays);

                await ExecuteScriptAsync(
                    connection,
                    "DELETE FROM [dbo].[IndexMonitoring] WHERE run_datetime < @DeleteDate AND database_name = @DatabaseName",
                    LoggerService,
                    new SqlParameter("@DeleteDate", deleteRecordsOlderThan),
                    new SqlParameter("@DatabaseName", databaseName));

                monitoringIndexLogCommand = @"
                    WITH CTE AS (
                        SELECT
                            *,
                            ROW_NUMBER() OVER (PARTITION BY create_tsql ORDER BY ID) AS RowNum
                        FROM [dbo].[IndexMonitoring])

                    SELECT *
                    FROM CTE
                    WHERE RowNum = 1 AND database_name = @DatabaseName;
                    ";

                using var reader = await ExecuteCommandAsync(connection, monitoringIndexLogCommand, LoggerService, new SqlParameter("@DatabaseName", databaseName));

                if (reader == null)
                {
                    await LoggerService.Error("ExecuteMaintenanceJob logging information error");
                    return;
                }

                var loggingInformation = IndexLog.GetAll(reader);

                await LoggerService.MonitoringInformation(loggingInformation, databaseName);
            }

            connection.Close();
        }

        public void InstallMaintenanceAndMonitoringScripts()
        {
            Task.Run(async () =>
            {
                using SqlConnection connection = new(Configuration.ConnectionString);
                connection.Open();

                foreach (var scriptName in Configuration.MaintenanceScripts)
                {
                    var assemblyScriptName = typeof(SqlServerJob).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(scriptName));

                    using var assemblyScriptStream = typeof(SqlServerJob).Assembly.GetManifestResourceStream(assemblyScriptName);
                    using var assemblyScriptStreamReader = new StreamReader(assemblyScriptStream);
                    var scriptContent = assemblyScriptStreamReader.ReadToEnd();

                    await ExecuteScriptAsync(connection, scriptContent, LoggerService);
                }

                foreach (var scriptName in Configuration.MonitoringScripts)
                {
                    var assemblyScriptName = typeof(SqlServerJob).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(scriptName));

                    using var assemblyScriptStream = typeof(SqlServerJob).Assembly.GetManifestResourceStream(assemblyScriptName);
                    using var assemblyScriptStreamReader = new StreamReader(assemblyScriptStream);
                    var scriptContent = assemblyScriptStreamReader.ReadToEnd();

                    await ExecuteScriptAsync(connection, scriptContent, LoggerService);
                }

                connection.Close();
            });
        }
    }
}
