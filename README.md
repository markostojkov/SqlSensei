<div align="center">
  <h3 align="center">SqlSensei</h3>

  <p align="center">
    C# .NET Library for maintenance and monitoring of databases
  </p>
</div>

## About The Project

SqlSensei is a C# .NET Library designed to assist with the maintenance and monitoring of databases. It provides a set of tools and utilities for database administrators and developers to manage and monitor database performance.

### Built With

- C#
- .NET Framework

<!-- GETTING STARTED -->

## Getting Started

<h4>Example with using Hangfire as scheduler for the jobs that monitor and do maintenance with default configuration</h4>

```
builder.Services.RegisterSqlSenseiSqlServer(SqlServerConfiguration.Default("connectionString","DatabaseToMonitorAndMaintenance1", "DatabaseToMonitorAndMaintenance2"));

app.UseSqlSenseiSqlServerHangfire(SqlSenseiHangfireOptions.Default);
```

<h4>These is an option to use endpoint logging meaning there will be endpoints you can send requests to and in case of errors with maintenance (index rebuilds, statistics update) and maintenance
priorities (high value missing index, etc...) you will get appropriate message back</h4>

```
builder.Services.RegisterSqlSenseiSqlServerEndpointLogging();

app.UseSqlSenseiSqlServerEndpointLogging(new SqlSenseiEndpointLoggerOptions()
{
    IndexInfoEndpoint = "/sql-sensei/index",
    MaintenanceInfoEndpoint = "/sql-sensei/maintenance"
});
```


## Installation

The package can be installed from NuGet

## Usage

<h4>If you want to use custom configuration for maintenance / monitoring</h4>

```
builder.Services.RegisterSqlSenseiSqlServer(SqlServerConfiguration.Create(
    "connectionString",
    30, // keep database logs for how many days
    SqlServerConfigurationMonitoringOptions.CoreNoQueryStore,
    SqlServerConfigurationMaintenanceOptions.OlaHallengrenDefault,
    "DatabaseToMonitorAndMaintenance1", "DatabaseToMonitorAndMaintenance2"));
```

## Roadmap

- [x] Add database maintenance
- [x] Add database high value missing index monitoring
- [ ] Add duplicate index monitoring


## Contributing


## License

Distributed under the MIT License.

## Contact

Marko Stojkov - [linkedin](https://www.linkedin.com/in/marko-stojkov/) - stojkovmarko1@gmail.com

## Acknowledgments

This project could not be done without the help of:
- Brent Ozar (author of all monitoring scripts used here - FirstResponderKit)
- Ola Hallengren (author of the maintenance script)
- HangfireIO