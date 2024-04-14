#nullable enable

using Newtonsoft.Json;
using System.Collections.Generic;

namespace SqlSensei.Core
{
    public class MonitoringLogRequest(
        IEnumerable<MonitoringJobIndexMissingLog> indexMissingLogs,
        IEnumerable<MonitoringJobIndexUsageLog> indexUsageLogs,
        IEnumerable<MonitoringJobServerLog> serverLogs,
        IEnumerable<MonitoringJobServerWaitStatLog> serverWaitStatLogs,
        IEnumerable<MonitoringJobServerFindingLog> serverFindingLogs)
    {
        public IEnumerable<MonitoringJobIndexMissingLog> IndexMissingLogs { get; } = indexMissingLogs;
        public IEnumerable<MonitoringJobIndexUsageLog> IndexUsageLogs { get; } = indexUsageLogs;
        public IEnumerable<MonitoringJobServerLog> ServerLogs { get; } = serverLogs;
        public IEnumerable<MonitoringJobServerWaitStatLog> ServerWaitStatLogs { get; } = serverWaitStatLogs;
        public IEnumerable<MonitoringJobServerFindingLog> ServerFindingLogs { get; } = serverFindingLogs;
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
        long writeUsage)
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
    }

    public class MonitoringJobServerLog(
        string? databaseName,
        short priority,
        int checkId,
        string details)
    {
        [JsonProperty("databaseName")]
        public string? DatabaseName { get; } = databaseName;

        [JsonProperty("priority")]
        public short Priority { get; } = priority;

        [JsonProperty("checkId")]
        public int CheckId { get; } = checkId;

        [JsonProperty("details")]
        public string Details { get; } = details;
    }

    public class MonitoringJobServerWaitStatLog(string type, long timeInMs)
    {
        [JsonProperty("type")]
        public string Type { get; } = type;

        [JsonProperty("timeInMs")]
        public long TimeInMs { get; } = timeInMs;
    }

    public class MonitoringJobServerFindingLog(int checkId, short priority, string details)
    {
        [JsonProperty("checkId")]
        public int CheckId { get; } = checkId;

        [JsonProperty("priority")]
        public short Priority { get; } = priority;

        [JsonProperty("details")]
        public string Details { get; } = details;
    }
}