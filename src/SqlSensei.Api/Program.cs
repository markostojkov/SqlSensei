using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Insights;
using SqlSensei.Api.Services;
using SqlSensei.Api.Storage;
using SqlSensei.Api.User;

namespace SqlSensei.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _ = builder.Services.AddControllers();

            _ = builder.Services.AddDbContext<SqlSenseiDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlSenseiDb"), opt => opt.EnableRetryOnFailure()));
            _ = builder.Services.AddScoped<CurrentCompanyService>();
            _ = builder.Services.AddScoped<StoreLogsToDatabaseService>();
            _ = builder.Services.AddScoped<JobService>();
            _ = builder.Services.AddScoped<SqlServerInsights>();
            _ = builder.Services.AddScoped<ServersService>();
            _ = builder.Services.AddScoped<CurrentUser>(x => new CurrentUser(x.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User));
            _ = builder.Services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            _ = builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                {
                    builder.Configuration.Bind("AzureAdB2C", options);

                    options.TokenValidationParameters.NameClaimType = "name";
                }, options => { builder.Configuration.Bind("AzureAdB2C", options); });

            _ = builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                    .WithOrigins("*")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });

            var app = builder.Build();

            _ = app.UseCors();

            _ = app.UseAuthentication();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            _ = app.UseCurrentCompanyMiddleware();

#if DEBUG
            using (var serviceScope = app.Services.CreateScope())
            {
                serviceScope?.ServiceProvider.GetRequiredService<SqlSenseiDbContext>().SeedData();
            }
#endif

            app.Run();

        }
    }
}
