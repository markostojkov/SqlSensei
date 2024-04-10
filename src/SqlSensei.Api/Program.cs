using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Services;
using SqlSensei.Api.Storage;

namespace SqlSensei.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _ = builder.Services.AddControllers();

            _ = builder.Services.AddDbContext<SqlSenseiDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlSenseiDb")));
            _ = builder.Services.AddScoped<CurrentCompanyService>();
            _ = builder.Services.AddScoped<StoreLogsToDatabaseService>();
            _ = builder.Services.AddScoped<JobService>();
            _ = builder.Services.AddApiVersioning(options =>
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

            var app = builder.Build();

            _ = app.UseAuthorization();

            _ = app.MapControllers();

            _ = app.UseCurrentCompanyMiddleware();
            
            app.Run();
        }
    }
}
