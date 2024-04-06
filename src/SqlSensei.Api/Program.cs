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

            var app = builder.Build();

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.UseCurrentCompanyMiddleware();

            _ = app.MapControllers();

            app.Run();
        }
    }
}
