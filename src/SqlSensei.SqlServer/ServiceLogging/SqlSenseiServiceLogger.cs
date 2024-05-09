using Newtonsoft.Json;
using SqlSensei.Core;
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
            HttpClient.BaseAddress = EnvHelpers.ApiUri(configuration.ApiVersion);
            HttpClient.DefaultRequestHeaders.Add("sqlsensei-token", configuration.ApiKey);
            ErrorLogger = errorLogger;
        }

        public ISqlSenseiErrorLoggerService ErrorLogger { get; }

        public async Task<Result<CanExecuteJobsResponse>> GetCanExecuteJobs()
        {
            try
            {
                var response = await HttpClient.GetAsync("can-execute-jobs");

                var x = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorLogger.Error(x);

                    return Result.Invalid<CanExecuteJobsResponse>(x);
                }

                return JsonConvert.DeserializeObject<Result<CanExecuteJobsResponse>>(x);
            }
            catch (System.Exception e)
            {
                ErrorLogger.Error(e.Message);

                return Result.Invalid<CanExecuteJobsResponse>(e.Message);
            }
        }

        public async Task LogMaintenance(long jobId, IEnumerable<IMaintenanceJobLog> logs)
        {
            try
            {
                var request = new MaintenanceLogRequest(
                    logs.Select(x => new MaintenanceLog(
                        x.DatabaseName,
                        x.Index,
                        x.Statistic,
                        x.IsError,
                        x.ErrorMessage))
                        .ToList());

                var jsonContent = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync($"jobs/{jobId}/maintenance", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var x = await response.Content.ReadAsStringAsync();

                    ErrorLogger.Error(x);
                }
            }
            catch (System.Exception e)
            {
                ErrorLogger.Error(e.Message);
            }
        }

        public async Task LogMonitoring(
            long jobId,
            IEnumerable<IMonitoringJobIndexMissingLog> logsMissingIndex,
            IEnumerable<IMonitoringJobIndexUsageLog> logsUsageIndex,
            IEnumerable<IMonitoringJobServerLog> logsServer,
            IEnumerable<IMonitoringJobServerPerformanceLogWaitStat> logsWaitStatsServer,
            IEnumerable<IMonitoringJobServerPerformanceLogFinding> logsFindingsServer,
            IEnumerable<IMonitoringJobQueryLog> queryLogs)
        {
            try
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
                        x.WriteUsage)),
                    logsServer.Select(x => new MonitoringJobServerLog(
                        x.DatabaseName,
                        x.Priority,
                        x.CheckId,
                        x.Details)),
                    logsWaitStatsServer.Select(x => new MonitoringJobServerWaitStatLog(
                        x.Type,
                        x.TimeInMs)),
                    logsFindingsServer.Select(x => new MonitoringJobServerFindingLog(
                        x.CheckId,
                        x.Priority,
                        x.Details,
                        x.Finding)),
                    MapQueryLogs(queryLogs));

                var jsonContent = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await HttpClient.PostAsync($"jobs/{jobId}/monitoring", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var x = await response.Content.ReadAsStringAsync();

                    ErrorLogger.Error(x);
                }
            }
            catch (System.Exception e)
            {
                ErrorLogger.Error(e.Message);
            }
        }

        private static IEnumerable<MonitoringJobQueryLog> MapQueryLogs(IEnumerable<IMonitoringJobQueryLog> queryLogs)
        {
            return queryLogs.Select(log => new MonitoringJobQueryLog(
                log.WaitType,
                log.TopNo,
                log.DatabaseName,
                log.QueryPlanCost,
                log.QueryText,
                log.Warnings,
                log.QueryPlan,
                log.MissingIndexes,
                log.ImplicitConversionInfo,
                log.ExecutionCount,
                log.ExecutionsPerMinute,
                log.TotalCPU,
                log.AverageCPU,
                log.TotalDuration,
                log.AverageDuration,
                log.TotalReads,
                log.AverageReads,
                log.TotalReturnedRows,
                log.AverageReturnedRows,
                log.MinReturnedRows,
                log.MaxReturnedRows,
                log.NumberOfPlans,
                log.NumberOfDistinctPlans,
                log.LastExecutionTime,
                log.QueryHash));
        }
    }
}