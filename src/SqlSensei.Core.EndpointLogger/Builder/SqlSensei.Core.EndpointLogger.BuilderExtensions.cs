using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.EndpointLogger
{
    public static class SqlSenseiSqlServerEndpointLoggerBuilderExtensions
    {
        public static IApplicationBuilder UseSqlSenseiSqlServerEndpointLogging(this IApplicationBuilder app, SqlSenseiEndpointLoggerOptions options)
        {
            app.Map(new PathString(options.IndexInfoEndpoint), delegate (IApplicationBuilder x)
            {
                x.UseMiddleware<SqlSenseiIndexInfoMiddleware>();
            });

            app.Map(new PathString(options.MaintenanceInfoEndpoint), delegate (IApplicationBuilder x)
            {
                x.UseMiddleware<SqlSenseiMaintenanceInfoMiddleware>();
            });

            return app;
        }

        public static IServiceCollection RegisterSqlSenseiSqlServerEndpointLogging(this IServiceCollection services)
        {
            services.TryAddSingleton<ISqlSenseiLoggerService, SqlSenseiSqlServerLoggerServiceEndpoint>();
            services.AddSingleton<SqlSenseiSqlServerLoggerServiceEndpoint>();

            return services;
        }
    }
}
