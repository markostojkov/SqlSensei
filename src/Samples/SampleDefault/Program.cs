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
                "F556BFF2-872A-42BC-A1E0-4324AFB7F453",
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
