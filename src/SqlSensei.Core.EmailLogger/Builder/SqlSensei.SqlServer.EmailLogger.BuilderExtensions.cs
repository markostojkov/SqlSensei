using Microsoft.Extensions.DependencyInjection;

using SqlSensei.Core;
using SqlSensei.Core.Logging.Email;

namespace SqlSensei.SqlServer.EmailLogger.Builder
{
    public static class SqlSenseiSqlServerEmailLoggerBuilderExtensions
    {
        public static IServiceCollection RegisterSqlSenseiEmailLogger(this IServiceCollection services, IEmailService emailService)
        {
            services.AddSingleton(emailService);
            services.AddSingleton<ISqlSenseiLoggerService, SqlSenseiLoggerServiceEmail>();

            return services;
        }
    }
}
