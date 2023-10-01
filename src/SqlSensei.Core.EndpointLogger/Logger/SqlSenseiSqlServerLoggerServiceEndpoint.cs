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

        public SqlSenseiSqlServerLoggerServiceEndpoint(ISqlSenseiConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ISqlSenseiConfiguration Configuration { get; }

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
            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

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

            await ExecuteScriptAsync(connection, sql, this,
                new SqlParameter("@MaintenanceTable", maintenanceInfoSqlTableName),
                new SqlParameter("@DatabaseName", database));

            string insertQuery = $"INSERT INTO {maintenanceInfoSqlTableName} ([Index], [Statistic], [IsError], [ErrorMessage], [DatabaseName]) VALUES (@Index, @Statistic, @IsError, @ErrorMessage, @DatabaseName)";

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                foreach (var log in jobLogs)
                {
                    using SqlCommand command = new(insertQuery, connection, transaction);
                    command.Parameters.AddWithValue("@Index", string.IsNullOrWhiteSpace(log.Index) ? DBNull.Value : log.Index);
                    command.Parameters.AddWithValue("@Statistic", log.Statistic);
                    command.Parameters.AddWithValue("@IsError", log.IsError);
                    command.Parameters.AddWithValue("@ErrorMessage", string.IsNullOrWhiteSpace(log.ErrorMessage) ? DBNull.Value : log.ErrorMessage);
                    command.Parameters.AddWithValue("@DatabaseName", database);

                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            connection.Close();
        }

        public async Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> indexLogs, string database)
        {
            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

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

            await ExecuteScriptAsync(connection, sql, this,
                new SqlParameter("@IndexTable", indexInfoSqlTableName),
                new SqlParameter("@DatabaseName", database));

            string insertQuery = $"INSERT INTO {indexInfoSqlTableName} ([DatabaseName], [TableName], [MagicBenefitNumber], [Impact], [IndexDetails]) VALUES (@DatabaseName, @TableName, @MagicBenefitNumber, @Impact, @IndexDetails)";

            using SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                foreach (var log in indexLogs)
                {
                    using SqlCommand command = new(insertQuery, connection, transaction);
                    command.Parameters.AddWithValue("@DatabaseName", log.DatabaseName);
                    command.Parameters.AddWithValue("@TableName", log.TableName);
                    command.Parameters.AddWithValue("@MagicBenefitNumber", log.MagicBenefitNumber);
                    command.Parameters.AddWithValue("@Impact", log.Impact);
                    command.Parameters.AddWithValue("@IndexDetails", log.IndexDetails);

                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            connection.Close();
        }

        public async Task<List<IMaintenanceJobLog>> GetMaintenanceLogs()
        {
            List<IMaintenanceJobLog> maintenanceLogs = new();

            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            string selectQuery = $"SELECT * FROM {maintenanceInfoSqlTableName} ORDER BY DatabaseName";

            using var reader = await ExecuteCommandAsync(connection, selectQuery, this);

            if (reader == null)
            {
                await Error("GetMaintenanceLogs information error");

                return maintenanceLogs;
            }

            while (reader.Read())
            {
                maintenanceLogs.Add(MaintenanceJobLog.MapFromDataReader(reader));
            }

            connection.Close();

            return maintenanceLogs;
        }

        public async Task<List<IMonitoringJobIndexLog>> GetIndexMonitoringLogs()
        {
            List<IMonitoringJobIndexLog> monitoringLogs = new();

            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            string selectQuery = $"SELECT * FROM {indexInfoSqlTableName} ORDER BY DatabaseName";

            using var reader = await ExecuteCommandAsync(connection, selectQuery, this);

            if (reader == null)
            {
                await Error("GetMonitoringLogs information error");

                return monitoringLogs;
            }

            while (reader.Read())
            {
                monitoringLogs.Add(IndexJobEndpointInfoLog.MapFromDataReader(reader));
            }

            connection.Close();

            return monitoringLogs;
        }
    }
}
