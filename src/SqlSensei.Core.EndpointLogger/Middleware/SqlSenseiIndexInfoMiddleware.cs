using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class SqlSenseiIndexInfoMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint;

        public SqlSenseiIndexInfoMiddleware(RequestDelegate next, SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint)
        {
            _next = next;
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
