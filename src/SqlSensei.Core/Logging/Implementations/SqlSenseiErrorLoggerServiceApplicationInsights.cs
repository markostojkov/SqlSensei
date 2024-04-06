using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

using System;

namespace SqlSensei.Core
{
    public class SqlSenseiErrorLoggerServiceApplicationInsights : ISqlSenseiErrorLoggerService
    {
        private readonly TelemetryClient telemetryClient;

        public SqlSenseiErrorLoggerServiceApplicationInsights()
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = @"InstrumentationKey=62e9e73f-c9a9-44b1-a821-035e9bf992bf;IngestionEndpoint=https://germanywestcentral-1.in.applicationinsights.azure.com/;LiveEndpoint=https://germanywestcentral.livediagnostics.monitor.azure.com/";
            telemetryClient = new TelemetryClient(config);
        }

        public void Error(Exception exception, string message)
        {
            telemetryClient.TrackException(exception);
            telemetryClient.TrackTrace(message, SeverityLevel.Error);
        }

        public void Error(string message)
        {
            telemetryClient.TrackTrace(message, SeverityLevel.Error);
            telemetryClient.TrackException(new SystemException(message));
        }
    }
}
