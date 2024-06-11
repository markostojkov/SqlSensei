import { Component } from '@angular/core';

@Component({
  selector: 'app-how-to-implement-dialog',
  templateUrl: './how-to-implement-dialog.component.html',
  styleUrls: ['./how-to-implement-dialog.component.css'],
})
export class HowToImplementDialogComponent {
  data = `
  public class Program
  {
      public static void Main(string[] args)
      {
          var builder = WebApplication.CreateBuilder(args);

          builder.Services.RegisterSqlSenseiUsingSqlServer(SqlServerConfiguration.Default(
              "5D64611D-86AC-4095-B7C3-3060DD5F8C06", //SqlSensei API KEY found when you create new Server
              [
                  new SqlSensei.Core.SqlSenseiConfigurationDatabase("Db1", "Data Source=.;Initial Catalog=Db1;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application"), // Database to do maintenance to
                  new SqlSensei.Core.SqlSenseiConfigurationDatabase("Db2", "Data Source=.;Initial Catalog=Db2;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application") // Database to do maintenance to
              ],
              "Data Source=.;Initial Catalog=Monitoring;Trusted_Connection=True; TrustServerCertificate=True;Integrated Security=True;Application Name=Application" // Connection string with sysadmin privilege to execute monitoring
              ));

          var app = builder.Build();

          _ = app.UseSqlSenseiUsingSqlServer();

          app.Run();
      }
  }
  `;

  constructor() {}
}
