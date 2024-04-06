using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.Core
{
    public class MonitoringLogRequest(
        IEnumerable<MonitoringJobIndexMissingLog> indexMissingLogs,
        IEnumerable<MonitoringJobIndexUsageLog> indexUsageLogs)
    {
        public IEnumerable<MonitoringJobIndexMissingLog> IndexMissingLogs { get; } = indexMissingLogs;
        public IEnumerable<MonitoringJobIndexUsageLog> IndexUsageLogs { get; } = indexUsageLogs;
    }

    public class MonitoringJobIndexMissingLog(
        string databaseName,
        string tableName,
        long magicBenefitNumber,
        string impact,
        string indexDetails)
    {
        [JsonProperty("databaseName")]
        public string DatabaseName { get; } = databaseName;

        [JsonProperty("tableName")]
        public string TableName { get; } = tableName;

        [JsonProperty("magicBenefitNumber")]
        public long MagicBenefitNumber { get; } = magicBenefitNumber;

        [JsonProperty("impact")]
        public string Impact { get; } = impact;

        [JsonProperty("indexDetails")]
        public string IndexDetails { get; } = indexDetails;
    }

    public class MonitoringJobIndexUsageLog(
        string databaseName,
        bool isClusteredIndex,
        string indexName,
        string tableName,
        string indexDetails,
        string usage,
        long readsUsage,
        long writeUsage,
        string userMessage,
        IEnumerable<string> indexColumns,
        IEnumerable<string> indexIncludeColumns)
    {
        [JsonProperty("databaseName")]
        public string DatabaseName { get; } = databaseName;

        [JsonProperty("isClusteredIndex")]
        public bool IsClusteredIndex { get; } = isClusteredIndex;

        [JsonProperty("indexName")]
        public string IndexName { get; } = indexName;

        [JsonProperty("tableName")]
        public string TableName { get; } = tableName;

        [JsonProperty("indexDetails")]
        public string IndexDetails { get; } = indexDetails;

        [JsonProperty("usage")]
        public string Usage { get; } = usage;

        [JsonProperty("readsUsage")]
        public long ReadsUsage { get; } = readsUsage;

        [JsonProperty("writeUsage")]
        public long WriteUsage { get; } = writeUsage;

        [JsonProperty("userMessage")]
        public string UserMessage { get; set; } = userMessage;

        [JsonProperty("indexColumns")]
        public List<string> IndexColumns { get; set; } = indexColumns.ToList();

        [JsonProperty("indexIncludeColumns")]
        public List<string> IndexIncludeColumns { get; set; } = indexIncludeColumns.ToList();
    }
}
