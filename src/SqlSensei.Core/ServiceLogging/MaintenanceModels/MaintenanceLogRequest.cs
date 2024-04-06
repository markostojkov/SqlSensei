using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.Core
{
    public class MaintenanceLogRequest(IEnumerable<MaintenanceLog> logs)
    {
        public List<MaintenanceLog> Logs { get; } = logs.ToList();
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