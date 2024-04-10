using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SqlSensei.Core;

namespace SqlSensei.SqlServer
{
    public static class RegisterSqlSenseiSqlServer
    {
        public static void RegisterSqlSenseiUsingSqlServer(this IServiceCollection services, SqlServerConfiguration options)
        {
            _ = services.AddHttpClient();
            _ = services.AddSingleton(options);
            _ = services.AddSingleton<ISqlSenseiConfiguration>(options);
            _ = services.AddSingleton<IServiceLogger, SqlSenseiServiceLogger>();
            _ = services.AddSingleton<ISqlSenseiJob, SqlServerJob>();

            if (EnvHelpers.IsRelease() && options.ReportErrorsToSqlSensei)
            {
                _ = services.AddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerServiceApplicationInsights>();
            }
            else
            {
                _ = services.AddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerServiceConsole>();
            }
        }

        public static IApplicationBuilder UseSqlSenseiUsingSqlServer(this IApplicationBuilder app)
        {
            var sqlServerJobService = app.ApplicationServices.GetService<ISqlSenseiJob>();

            sqlServerJobService.InstallMaintenanceAndMonitoringScripts();

            sqlServerJobService.StartService();

            return app;
        }
    }
}