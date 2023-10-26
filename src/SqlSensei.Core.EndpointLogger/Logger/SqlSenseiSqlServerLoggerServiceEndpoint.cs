using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class SqlSenseiSqlServerLoggerServiceEndpoint : SqlServerBase, ISqlSenseiLoggerService
    {
        private const string maintenanceInfoSqlTableName = "MaintenanceTableEndpointInfo";
        private const string indexInfoSqlTableName = "IndexTableEndpointInfo";
        private const string indexUsageInfoSqlTableName = "IndexUsageTableEndpointInfo";

        public SqlSenseiSqlServerLoggerServiceEndpoint(ISqlSenseiConfiguration configuration) : base(configuration)
        {
            Configuration = configuration;
            LoggerService = this;
        }

        public Task Error(Exception exception, string message)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public Task Error(string message)
        {
            Console.Error.WriteLine(message);
            return Task.CompletedTask;
        }

        public async Task MaintenanceInformation(IEnumerable<IMaintenanceJobLog> jobLogs, string database)
        {
            var sql = @$"
                IF OBJECT_ID(@MaintenanceTable, 'U') IS NULL
                BEGIN
                    CREATE TABLE {maintenanceInfoSqlTableName} (
                        [Index] NVARCHAR(MAX) NULL,
                        [Statistic] NVARCHAR(MAX),
                        [IsError] BIT,
                        [ErrorMessage] NVARCHAR(MAX) NULL,
                        [DatabaseName] NVARCHAR(MAX)
                    )
                END
                ELSE
                BEGIN
                    DELETE FROM {maintenanceInfoSqlTableName} WHERE DatabaseName = @DatabaseName;
                END
                ";

            await ExecuteCommandAsync(sql, new SqlParameter("@MaintenanceTable", maintenanceInfoSqlTableName), new SqlParameter("@DatabaseName", database));

            string insertQuery = $"INSERT INTO {maintenanceInfoSqlTableName} ([Index], [Statistic], [IsError], [ErrorMessage], [DatabaseName]) VALUES (@Index, @Statistic, @IsError, @ErrorMessage, @DatabaseName)";

            foreach (var log in jobLogs)
            {
                await ExecuteCommandAsync(
                    insertQuery,
                    new SqlParameter("@Index", string.IsNullOrWhiteSpace(log.Index) ? DBNull.Value : log.Index),
                    new SqlParameter("@Statistic", log.Statistic),
                    new SqlParameter("@IsError", log.IsError),
                    new SqlParameter("@ErrorMessage", string.IsNullOrWhiteSpace(log.ErrorMessage) ? DBNull.Value : log.ErrorMessage),
                    new SqlParameter("@DatabaseName", database));
            }
        }

        public async Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> indexLogs, IEnumerable<IMonitoringJobIndexLogUsage> indexLogsUsage, string database)
        {
            var sql = @$"
                IF OBJECT_ID(@IndexTable, 'U') IS NULL
                BEGIN
                    CREATE TABLE {indexInfoSqlTableName} (
                        [DatabaseName] NVARCHAR(MAX),
                        [TableName] NVARCHAR(MAX),
                        [MagicBenefitNumber] BIGINT,
                        [Impact] NVARCHAR(MAX),
                        [IndexDetails] NVARCHAR(MAX)
                    )
                END
                ELSE
                BEGIN
                    DELETE FROM {indexInfoSqlTableName} WHERE DatabaseName = @DatabaseName;
                END
            ";

            await ExecuteCommandAsync(sql, new SqlParameter("@IndexTable", indexInfoSqlTableName), new SqlParameter("@DatabaseName", database));

            string insertQuery = $"INSERT INTO {indexInfoSqlTableName} ([DatabaseName], [TableName], [MagicBenefitNumber], [Impact], [IndexDetails]) VALUES (@DatabaseName, @TableName, @MagicBenefitNumber, @Impact, @IndexDetails)";

            foreach (var log in indexLogs)
            {
                await ExecuteCommandAsync(
                    insertQuery,
                    new SqlParameter("@DatabaseName", log.DatabaseName),
                    new SqlParameter("@TableName", log.TableName),
                    new SqlParameter("@MagicBenefitNumber", log.MagicBenefitNumber),
                    new SqlParameter("@Impact", log.Impact),
                    new SqlParameter("@IndexDetails", log.IndexDetails));
            }

            sql = @$"
                IF OBJECT_ID(@IndexTable, 'U') IS NULL
                BEGIN
                    CREATE TABLE {indexUsageInfoSqlTableName} (
                        [DatabaseName]      NVARCHAR(MAX),
                        [IndexName]         NVARCHAR(MAX),
                        [TableName]         NVARCHAR(MAX),
                        [IndexDetails]      NVARCHAR(MAX),
                        [Usage]             NVARCHAR(MAX),
                        [ReadsUsage]        BIGINT,
                        [WriteUsage]        BIGINT,
                        [UserMessage]       NVARCHAR(MAX),
                        [IsClusteredIndex]  BIT
                    )
                END
                ELSE
                BEGIN
                    DELETE FROM {indexUsageInfoSqlTableName} WHERE DatabaseName = @DatabaseName;
                END
            ";

            await ExecuteCommandAsync(sql, new SqlParameter("@IndexTable", indexUsageInfoSqlTableName), new SqlParameter("@DatabaseName", database));

            insertQuery = $"INSERT INTO {indexUsageInfoSqlTableName} ([DatabaseName], [IndexName], [TableName], [IndexDetails], [Usage], [ReadsUsage], [WriteUsage], [UserMessage], [IsClusteredIndex]) VALUES (@DatabaseName, @IndexName, @TableName, @IndexDetails, @Usage, @ReadsUsage, @WriteUsage, @UserMessage, @IsClusteredIndex)";

            foreach (var log in indexLogsUsage)
            {
                await ExecuteCommandAsync(
                    insertQuery,
                    new SqlParameter("@DatabaseName", log.DatabaseName),
                    new SqlParameter("@IndexName", log.IndexName),
                    new SqlParameter("@TableName", log.TableName),
                    new SqlParameter("@IndexDetails", log.IndexDetails),
                    new SqlParameter("@Usage", log.Usage),
                    new SqlParameter("@ReadsUsage", log.ReadsUsage),
                    new SqlParameter("@WriteUsage", log.WriteUsage),
                    new SqlParameter("@UserMessage", log.UserMessage),
                    new SqlParameter("@IsClusteredIndex", log.IsClusteredIndex));
            }
        }

        public async Task<List<IMaintenanceJobLog>> GetMaintenanceLogs()
        {
            List<IMaintenanceJobLog> maintenanceLogs = new();

            var result = await ExecuteCommandAsync(
                $"SELECT * FROM {maintenanceInfoSqlTableName} ORDER BY DatabaseName",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        maintenanceLogs.Add(MaintenanceJobLog.MapFromDataReader(reader));
                    }
                });

            if (!result)
            {
                await Error("GetMaintenanceLogs information error");

                return maintenanceLogs;
            }

            return maintenanceLogs;
        }

        public async Task<List<IMonitoringJobIndexLog>> GetIndexMonitoringLogs()
        {
            List<IMonitoringJobIndexLog> monitoringLogs = new();

            var result = await ExecuteCommandAsync(
                $"SELECT * FROM {indexInfoSqlTableName} ORDER BY DatabaseName",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        monitoringLogs.Add(IndexJobEndpointInfoLog.MapFromDataReader(reader));
                    }
                });

            if (!result)
            {
                await Error("GetMonitoringLogs information error");

                return monitoringLogs;
            }

            return monitoringLogs;
        }
    }
}
