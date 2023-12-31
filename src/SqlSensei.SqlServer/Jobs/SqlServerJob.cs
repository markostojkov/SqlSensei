﻿using System;
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
        public ISqlSenseiLoggerService LoggerService { get; }

        public SqlServerJob(ISqlSenseiConfiguration configuration, ISqlSenseiLoggerService loggerService, ISqlSenseiErrorLoggerService errorLoggerService) : base(errorLoggerService, configuration)
        {
            LoggerService = loggerService;
        }

        public async Task ExecuteMaintenanceJob()
        {
            await ExecuteCommandAsyncNoTransaction(Configuration.GetMaintenanceScript());

            // Go through all databases in configuration, DELETE older rows, do maintenance and log
            foreach (var databaseName in Configuration.Databases)
            {
                var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.DropLogsOlderThanDays);

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
                    await ErrorLoggerService.Error("ExecuteMaintenanceJob logging information error");
                    return;
                }

                await LoggerService.MaintenanceInformation(loggingInformation, databaseName);
            }
        }

        public async Task ExecuteMonitoringLogJob()
        {
            await ExecuteScriptAsyncGoStatements(Configuration.GetMonitoringLog());
        }

        public async Task ExecuteMonitoringJob()
        {
            foreach (var databaseName in Configuration.Databases)
            {
                var indexLogMissing = await ExecuteMonitoringLogMissingIndexes(databaseName);
                var indexLogUsage = await ExecuteMonitoringLogUsageIndexes(databaseName);

                await LoggerService.MonitoringInformation(indexLogMissing, indexLogUsage, databaseName);
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

        private async Task<List<IndexLogUsage>> ExecuteMonitoringLogUsageIndexes(string databaseName)
        {
            var loggingInformation = new List<IndexLogUsage>();

            var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.DropLogsOlderThanDays);

            await ExecuteCommandAsync(
                @"IF OBJECT_ID('[dbo].[IndexMonitoringUsage]', 'U') IS NOT NULL
                    BEGIN
                        DELETE FROM [dbo].[IndexMonitoringUsage] WHERE run_datetime < @DeleteDate AND database_name = @DatabaseName;
                    END",
                new SqlParameter("@DeleteDate", deleteRecordsOlderThan),
                new SqlParameter("@DatabaseName", databaseName));

            var monitoringIndexLogCommand = @"
                IF OBJECT_ID('[dbo].[IndexMonitoringUsage]', 'U') IS NOT NULL
                    BEGIN
                        WITH CTE AS (
                            SELECT
                                *,
                                ROW_NUMBER() OVER (PARTITION BY create_tsql ORDER BY ID) AS RowNum
                            FROM [dbo].[IndexMonitoringUsage])

                        SELECT *
                        FROM CTE
                        WHERE RowNum = 1 AND database_name = @DatabaseName;
                    END
                ";

            var result = await ExecuteCommandAsync(
                monitoringIndexLogCommand,
                (reader) =>
                {
                    loggingInformation = IndexLogUsage.GetAll(reader);
                },
                new SqlParameter("@DatabaseName", databaseName));

            if (!result)
            {
                await ErrorLoggerService.Error("ExecuteMonitoringJob logging information error");
            }

            return loggingInformation;
        }

        private async Task<List<IndexLog>> ExecuteMonitoringLogMissingIndexes(string databaseName)
        {
            var loggingInformation = new List<IndexLog>();

            var deleteRecordsOlderThan = DateTime.UtcNow.AddDays(-Configuration.DropLogsOlderThanDays);

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

            var result = await ExecuteCommandAsync(
                monitoringIndexLogCommand,
                (reader) =>
                {
                    loggingInformation = IndexLog.GetAll(reader);
                },
                new SqlParameter("@DatabaseName", databaseName));

            if (!result)
            {
                await ErrorLoggerService.Error("ExecuteMonitoringJob logging information error");
            }

            return loggingInformation;
        }
    }
}
