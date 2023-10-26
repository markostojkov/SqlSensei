using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class SqlSenseiIndexUsageInfoMiddleware
    {
        private readonly SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint;

        public SqlSenseiIndexUsageInfoMiddleware(SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint)
        {
            this.loggerServiceEndpoint = loggerServiceEndpoint;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var indexMonitoringData = await loggerServiceEndpoint.GetIndexMonitoringLogs();

            var hasErrors = indexMonitoringData.Any();

            context.Response.StatusCode = hasErrors ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK;

            context.Response.ContentType = "text/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(indexMonitoringData));
        }
    }
}
