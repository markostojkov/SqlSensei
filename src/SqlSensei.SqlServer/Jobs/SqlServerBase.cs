using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer
{
    public abstract class SqlServerBase
    {
        protected ISqlSenseiErrorLoggerService ErrorLoggerService { get; set; }
        protected ISqlSenseiConfiguration Configuration { get; set; }

        protected SqlServerBase(ISqlSenseiErrorLoggerService loggerService, ISqlSenseiConfiguration configuration)
        {
            ErrorLoggerService = loggerService;
            Configuration = configuration;
        }

        protected SqlServerBase(ISqlSenseiConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected async Task ExecuteScriptAsyncGoStatements(string scriptContent)
        {
            using SqlConnection connection = new(Configuration.ConnectionString);

            connection.Open();

            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                await ErrorLoggerService.Error("Script content is empty");
                return;
            }

            try
            {
                var commandStrings = Regex.Split(scriptContent, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                                          .Where(commandString => !string.IsNullOrWhiteSpace(commandString))
                                          .ToList();

                foreach (string commandString in commandStrings)
                {
                    using SqlCommand command = new(commandString, connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                await ErrorLoggerService.Error(e, "Error executing script");
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

        protected async Task ExecuteCommandAsync(string sql, params SqlParameter[] parameters)
        {
            await ExecuteCommandAsyncPrivate(sql, true, null, parameters);
        }

        protected async Task ExecuteCommandAsyncNoTransaction(string sql, params SqlParameter[] parameters)
        {
            await ExecuteCommandAsyncPrivate(sql, false, null, parameters);
        }

        private async Task<bool> ExecuteCommandAsyncPrivate(string sql, bool isTransactional, Action<SqlDataReader> commandAction, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                await ErrorLoggerService.Error("Script content is empty");
                return false;
            }

            using SqlConnection connection = new(Configuration.ConnectionString);

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

                    await ErrorLoggerService.Error($"SQL Execution Error: {ex.Message}");

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
                    await ErrorLoggerService.Error($"SQL Execution Error: {ex.Message}");

                    return false;
                }
            }
        }
    }
}
