using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class SqlSenseiMaintenanceInfoMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint;

        public SqlSenseiMaintenanceInfoMiddleware(RequestDelegate next, SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint)
        {
            this.next = next;
            this.loggerServiceEndpoint = loggerServiceEndpoint;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var maintenanceData = await loggerServiceEndpoint.GetMaintenanceLogs();

            var hasErrors = maintenanceData.Any(x => x.IsError);

            context.Response.ContentType = "text/json";

            if (hasErrors)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(JsonSerializer.Serialize(maintenanceData.Where(x => x.IsError)));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync(JsonSerializer.Serialize(maintenanceData));
            }
        }
    }
}
