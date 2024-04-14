using Newtonsoft.Json;
using System.Collections.Generic;

namespace SqlSensei.Core
{
    public class MaintenanceLogRequest
    {
        [JsonProperty("logs")]
        public List<MaintenanceLog> Logs { get; }

        public MaintenanceLogRequest(List<MaintenanceLog> logs)
        {
            Logs = logs;
        }
    }

    public class MaintenanceLog(string databaseName, string index, string statistic, bool isError, string errorMessage)
    {
        [JsonProperty("databaseName")]
        public string DatabaseName { get; } = databaseName;

        [JsonProperty("index")]
        public string Index { get; } = index;

        [JsonProperty("statistic")]
        public string Statistic { get; } = statistic;

        [JsonProperty("isError")]
        public bool IsError { get; } = isError;

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; } = errorMessage;
    }
}