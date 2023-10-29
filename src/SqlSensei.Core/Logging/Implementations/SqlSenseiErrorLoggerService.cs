using System;
using System.Threading.Tasks;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SqlSensei.Core
{
    public class SqlSenseiErrorLoggerService : ISqlSenseiErrorLoggerService
    {
        private readonly TelemetryClient telemetryClient;

        public SqlSenseiErrorLoggerService()
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = @"InstrumentationKey=62e9e73f-c9a9-44b1-a821-035e9bf992bf;IngestionEndpoint=https://germanywestcentral-1.in.applicationinsights.azure.com/;LiveEndpoint=https://germanywestcentral.livediagnostics.monitor.azure.com/";
            telemetryClient = new TelemetryClient(config);
        }

        public Task Error(Exception exception, string message)
        {
            telemetryClient.TrackException(exception);
            telemetryClient.TrackTrace(message, SeverityLevel.Error);

            return Task.CompletedTask;
        }

        public Task Error(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Error);
            telemetryClient.TrackException(new SystemException(message));

            return Task.CompletedTask;
        }
    }

}
