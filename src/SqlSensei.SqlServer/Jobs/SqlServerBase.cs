using Microsoft.Data.SqlClient;

using SqlSensei.Core;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSensei.SqlServer
{
    public abstract class SqlServerBase
    {
        protected ISqlSenseiErrorLoggerService ErrorLoggerService { get; set; }
        protected SqlServerConfiguration Configuration { get; set; }
        protected SqlConnectionStringBuilder ConnectionStringParsed { get; }

        protected SqlServerBase(ISqlSenseiErrorLoggerService loggerService, SqlServerConfiguration configuration)
        {
            ErrorLoggerService = loggerService;
            Configuration = configuration;
            ConnectionStringParsed = new SqlConnectionStringBuilder(configuration.MonitoringAndMaintenanceScriptDatabaseConnection);
        }

        protected async Task ExecuteScriptAsyncGoStatements(string scriptContent)
        {
            try
            {
                using SqlConnection connection = new(Configuration.MonitoringAndMaintenanceScriptDatabaseConnection);

                connection.Open();

                if (string.IsNullOrWhiteSpace(scriptContent))
                {
                    ErrorLoggerService.Error("Script content is empty");
                    return;
                }

                var commandStrings = Regex.Split(scriptContent, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                                          .Where(commandString => !string.IsNullOrWhiteSpace(commandString))
                                          .ToList();

                foreach (var commandString in commandStrings)
                {
                    using SqlCommand command = new(commandString, connection);
                    command.CommandTimeout = 240;
                    _ = await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                ErrorLoggerService.Error(e, "Error executing script");
            }
        }

        protected async Task<bool> ExecuteCommandAsync(string sql, Action<SqlDataReader> commandAction = null, params SqlParameter[] parameters)
        {
            return await ExecuteCommandAsyncPrivate(sql, true, commandAction, parameters);
        }

        protected async Task<bool> ExecuteCommandAsyncNoTransaction(string sql, Action<SqlDataReader> commandAction = null, params SqlParameter[] parameters)
        {
            return await ExecuteCommandAsyncPrivate(sql, false, commandAction, parameters);
        }

        protected async Task<bool> ExecuteCommandAsync(string sql, params SqlParameter[] parameters)
        {
            return await ExecuteCommandAsyncPrivate(sql, true, null, parameters);
        }

        protected async Task<bool> ExecuteCommandAsyncNoTransaction(string sql, params SqlParameter[] parameters)
        {
            return await ExecuteCommandAsyncPrivate(sql, false, null, parameters);
        }

        private async Task<bool> ExecuteCommandAsyncPrivate(string sql, bool isTransactional, Action<SqlDataReader> commandAction, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                ErrorLoggerService.Error("Script content is empty");
                return false;
            }

            using SqlConnection connection = new(Configuration.MonitoringAndMaintenanceScriptDatabaseConnection);

            await connection.OpenAsync();

            if (isTransactional)
            {
                using var transaction = connection.BeginTransaction();

                try
                {
                    using var command = new SqlCommand(sql, connection, transaction);
                    command.Parameters.AddRange(parameters);

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        commandAction?.Invoke(result);
                    }

                    transaction?.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();

                    ErrorLoggerService.Error($"SQL Execution Error: {ex.Message}");

                    return false;
                }
            }
            else
            {
                try
                {
                    using var command = new SqlCommand(sql, connection);
                    command.Parameters.AddRange(parameters);

                    using var result = await command.ExecuteReaderAsync();

                    commandAction?.Invoke(result);

                    return true;
                }
                catch (Exception ex)
                {
                    ErrorLoggerService.Error($"SQL Execution Error: {ex.Message}");

                    return false;
                }
            }
        }
    }
}