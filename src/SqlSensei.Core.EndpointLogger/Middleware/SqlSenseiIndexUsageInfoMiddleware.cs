﻿using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class SqlSenseiIndexUsageInfoMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint;

        public SqlSenseiIndexUsageInfoMiddleware(RequestDelegate next, SqlSenseiSqlServerLoggerServiceEndpoint loggerServiceEndpoint)
        {
            this.next = next;
            this.loggerServiceEndpoint = loggerServiceEndpoint;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var indexMonitoringData = await loggerServiceEndpoint.GetIndexUsageMonitoringLogs();

            var hasErrors = indexMonitoringData.Any();

            context.Response.StatusCode = hasErrors ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK;

            context.Response.ContentType = "text/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(indexMonitoringData));
        }
    }
}
