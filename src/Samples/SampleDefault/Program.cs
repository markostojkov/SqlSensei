using SqlSensei.SqlServer;

namespace SampleDefault
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            _ = builder.Services.AddAuthorization();

            builder.Services.RegisterSqlSenseiUsingSqlServer(SqlServerConfiguration.Default(
                "A20B0D25-4FB0-45CE-BE71-2BB9294CBAE3",
                [
                    new SqlSensei.Core.SqlSenseiConfigurationDatabase("Intrensic", "Data Source=.;Initial Catalog=Intrensic;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application"),
                    new SqlSensei.Core.SqlSenseiConfigurationDatabase("HangFire", "Data Source=.;Initial Catalog=HangFire;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application")
                ],
                "Data Source=.;Initial Catalog=Monitoring;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application"
                ));

            var app = builder.Build();

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthorization();

            _ = app.UseSqlSenseiUsingSqlServer();

            app.Run();
        }
    }
}
