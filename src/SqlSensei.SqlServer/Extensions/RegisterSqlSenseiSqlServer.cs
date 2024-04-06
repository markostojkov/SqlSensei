using Microsoft.Extensions.DependencyInjection;
using SqlSensei.Core;

namespace SqlSensei.SqlServer
{
    public static class RegisterSqlSenseiSqlServer
    {
        public static void RegisterSqlSenseiUsingSqlServer(this IServiceCollection services, SqlServerConfiguration options)
        {
            _ = services.AddHttpClient();
            _ = services.AddSingleton<ISqlSenseiConfiguration>(options);
            _ = services.AddSingleton<ISqlSenseiJob, SqlServerJob>();
            _ = services.AddSingleton<IServiceLogger, SqlSenseiServiceLogger>();

            if (EnvHelpers.IsRelease() && options.ReportErrorsToSqlSensei)
            {
                _ = services.AddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerServiceApplicationInsights>();
            }
            else
            {
                _ = services.AddSingleton<ISqlSenseiErrorLoggerService, SqlSenseiErrorLoggerServiceConsole>();
            }
        }
    }
}
