using Hangfire;
using Hangfire.SqlServer;

using Newtonsoft.Json;

using SqlSensei.SqlServer;
using SqlSensei.SqlServer.EndpointLogger;
using SqlSensei.SqlServer.Hangfire;
using SqlSensei.SqlServer.Hangfire.Builder;

namespace WebApplication1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services
                .RegisterSqlSenseiSqlServerEndpointLogging()
                .RegisterSqlSenseiSqlServer(SqlServerConfiguration.Default(
                "Data Source=.;Initial Catalog=Monitoring;Trusted_Connection=True; TrustServerCertificate=True;Application Name=Monitoring Application;multipleactiveresultsets=true",
                "Intrensic",
                "Hangfire"));

            builder.Services
            .AddHangfire(c =>
            {
                c.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseSerializerSettings(new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All
                })
                .UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection("Data Source=.;Initial Catalog=HangFire;Trusted_Connection=True; TrustServerCertificate=True;Application Name=Intrensic Application"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
            });

            builder.Services.AddHangfireServer(options => { });
            builder.Services.AddScoped<IBackgroundJobClient>(sp => new BackgroundJobClient(JobStorage.Current));

            var app = builder.Build();

            app.MapControllers();

            app.UseHangfireDashboard(pathMatch: "/tasks", options: new DashboardOptions { });

            app.UseSqlSenseiSqlServerHangfire(SqlSenseiHangfireOptions.Default)
               .UseSqlSenseiSqlServerEndpointLogging(new SqlSenseiEndpointLoggerOptions()
               {
                   IndexInfoEndpoint = "/sql-sensei/index",
                   IndexUsageInfoEndpoint = "/sql-sensei/index-usage",
                   MaintenanceInfoEndpoint = "/sql-sensei/maintenance"
               });

            app.Run();
        }
    }
}