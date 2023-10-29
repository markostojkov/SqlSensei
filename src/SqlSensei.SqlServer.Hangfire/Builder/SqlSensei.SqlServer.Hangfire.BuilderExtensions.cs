using Hangfire;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.Hangfire.Builder
{
    public static class SqlSenseiSqlServerHangfireBuilderExtensions
    {
        /// <summary>
        /// Configures and schedules recurring SQL Sensei maintenance, monitoring job using Hangfire. This method must be called after hangfire in the chain
        /// </summary>
        /// <param name="services">The service provider.</param>
        /// <param name="cron">The cron expression defining the job's execution schedule. Default is "0 0 12 ? 1/1 SAT#4 *" (every First Saturday of the month).</param>
        /// <param name="queue">The name of the queue to use for enqueuing the job. Default is the Hangfire default queue.</param>
        public static IApplicationBuilder UseSqlSenseiSqlServerHangfire(this IApplicationBuilder app, SqlSenseiHangfireOptions options)
        {
            RecurringJob.AddOrUpdate<ISqlSenseiJob>("ExecuteMaintenanceJob", options.Queue, sqlSenseiService => sqlSenseiService.ExecuteMaintenanceJob(), options.ExecuteMaintenanceJobCron);
            RecurringJob.AddOrUpdate<ISqlSenseiJob>("ExecuteMonitoringJob", options.Queue, sqlSenseiService => sqlSenseiService.ExecuteMonitoringJob(), options.ExecuteMonitoringJobCron);
            RecurringJob.AddOrUpdate<ISqlSenseiJob>("ExecuteMonitoringLogJob", options.Queue, sqlSenseiService => sqlSenseiService.ExecuteMonitoringLogJob(), options.ExecuteMonitoringLogJobCron);

            app.ApplicationServices.GetRequiredService<ISqlSenseiJob>().InstallMaintenanceAndMonitoringScripts();

            return app;
        }

        public static IServiceCollection RegisterSqlSenseiSqlServer(this IServiceCollection services, SqlServerConfiguration configuration)
        {
            services.AddSingleton<ISqlSenseiConfiguration>(configuration);
            services.AddSingleton<ISqlSenseiJob, SqlServerJob>();
            services.TryAddSingleton<ISqlSenseiLoggerService, SqlSenseiLoggerServiceConsole>();

#if Release
            services.TryAddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerService>();
#else
            services.TryAddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerServiceDebug>();
#endif

            return services;
        }
    }
}
