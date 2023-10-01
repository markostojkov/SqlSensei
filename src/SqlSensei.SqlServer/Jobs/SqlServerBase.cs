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
        protected async Task ExecuteScriptAsync(SqlConnection connection, string scriptContent, ISqlSenseiLoggerService loggerService)
        {
            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                await loggerService.Error("Script content is empty");
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
                await loggerService.Error(e, "Error executing script");
            }
        }

        protected async Task<SqlDataReader> ExecuteCommandAsync(SqlConnection connection, string sqlCommand, ISqlSenseiLoggerService loggerService)
        {
            if (string.IsNullOrWhiteSpace(sqlCommand))
            {
                await loggerService.Error("Script content is empty");
                return null;
            }

            try
            {
                using SqlCommand command = new(sqlCommand, connection);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                return reader;
            }
            catch (Exception e)
            {
                await loggerService.Error(e, "Error executing script");

                return null;
            }
        }

        protected async Task ExecuteScriptAsync(SqlConnection connection, string sql, ISqlSenseiLoggerService loggerService, params SqlParameter[] parameters)
        {
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                await loggerService.Error($"SQL Execution Error: {ex.Message}");
            }
        }

        protected async Task<SqlDataReader> ExecuteCommandAsync(SqlConnection connection, string sql, ISqlSenseiLoggerService loggerService, params SqlParameter[] parameters)
        {
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            try
            {
                return await command.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                await loggerService.Error($"SQL Execution Error: {ex.Message}");
                return null;
            }
        }
    }
}
