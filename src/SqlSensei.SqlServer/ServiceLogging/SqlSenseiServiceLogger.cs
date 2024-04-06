using Newtonsoft.Json;
using SqlSensei.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SqlSensei.SqlServer
{
    public class SqlSenseiServiceLogger : IServiceLogger
    {
        private readonly HttpClient HttpClient;

        public SqlSenseiServiceLogger(
            ISqlSenseiConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ISqlSenseiErrorLoggerService errorLogger)
        {
            HttpClient = httpClientFactory.CreateClient("SqlSenseiClient");
            HttpClient.BaseAddress = new Uri($"https://api.sqlsensei.net/sqlserver/{configuration.ApiVersion}");
            HttpClient.DefaultRequestHeaders.Add("sqlsensei-token", configuration.ApiKey);
            ErrorLogger = errorLogger;
        }

        public ISqlSenseiErrorLoggerService ErrorLogger { get; }

        public async Task LogMaintenance(IEnumerable<IMaintenanceJobLog> logs)
        {
            var request = new MaintenanceLogRequest(
                logs.Select(x => new MaintenanceLog(
                    x.DatabaseName,
                    x.Index,
                    x.Statistic,
                    x.IsError,
                    x.ErrorMessage)));

            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("maintenance/log", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var x = await response.Content.ReadAsStringAsync();

                ErrorLogger.Error(x);
            }
        }

        public async Task LogMonitoring(IEnumerable<IMonitoringJobIndexMissingLog> logsMissingIndex, IEnumerable<IMonitoringJobIndexUsageLog> logsUsageIndex)
        {
            var request = new MonitoringLogRequest(
                logsMissingIndex.Select(x => new MonitoringJobIndexMissingLog(
                    x.DatabaseName,
                    x.TableName,
                    x.MagicBenefitNumber,
                    x.Impact,
                    x.IndexDetails)),
                logsUsageIndex.Select(x => new MonitoringJobIndexUsageLog(
                    x.DatabaseName,
                    x.IsClusteredIndex,
                    x.IndexName,
                    x.TableName,
                    x.IndexDetails,
                    x.Usage,
                    x.ReadsUsage,
                    x.WriteUsage)));

            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("monitoring/log", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var x = await response.Content.ReadAsStringAsync();

                ErrorLogger.Error(x);
            }
        }
    }
}