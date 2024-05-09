#nullable enable

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SqlSensei.Core
{
    public class MonitoringLogRequest(
        IEnumerable<MonitoringJobIndexMissingLog> indexMissingLogs,
        IEnumerable<MonitoringJobIndexUsageLog> indexUsageLogs,
        IEnumerable<MonitoringJobServerLog> serverLogs,
        IEnumerable<MonitoringJobServerWaitStatLog> serverWaitStatLogs,
        IEnumerable<MonitoringJobServerFindingLog> serverFindingLogs,
        IEnumerable<MonitoringJobQueryLog> queryLogs)
    {
        public IEnumerable<MonitoringJobIndexMissingLog> IndexMissingLogs { get; } = indexMissingLogs;
        public IEnumerable<MonitoringJobIndexUsageLog> IndexUsageLogs { get; } = indexUsageLogs;
        public IEnumerable<MonitoringJobServerLog> ServerLogs { get; } = serverLogs;
        public IEnumerable<MonitoringJobServerWaitStatLog> ServerWaitStatLogs { get; } = serverWaitStatLogs;
        public IEnumerable<MonitoringJobServerFindingLog> ServerFindingLogs { get; } = serverFindingLogs;
        public IEnumerable<MonitoringJobQueryLog> QueryLogs { get; } = queryLogs;
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

    public class MonitoringJobServerFindingLog(int checkId, short priority, string details, string finding)
    {
        [JsonProperty("checkId")]
        public int CheckId { get; } = checkId;

        [JsonProperty("priority")]
        public short Priority { get; } = priority;

        [JsonProperty("details")]
        public string Details { get; } = details;

        [JsonProperty("finding")]
        public string Finding { get; } = finding;
    }

    public class MonitoringJobQueryLog(
        string waitType,
        int topNo,
        string databaseName,
        float? queryPlanCost,
        string queryText,
        string warnings,
        string queryPlan,
        string missingIndexes,
        string implicitConversionInfo,
        long? executionCount,
        decimal? executionsPerMinute,
        long? totalCPU,
        long? averageCPU,
        long? totalDuration,
        long? averageDuration,
        long? totalReads,
        long? averageReads,
        long? totalReturnedRows,
        decimal? averageReturnedRows,
        long? minReturnedRows,
        long? maxReturnedRows,
        int? numberOfPlans,
        int? numberOfDistinctPlans,
        DateTime? lastExecutionTime,
        byte[]? queryHash)
    {
        [JsonProperty("waitType")]
        public string WaitType { get; } = waitType;

        [JsonProperty("topNo")]
        public int TopNo { get; } = topNo;

        [JsonProperty("databaseName")]
        public string DatabaseName { get; } = databaseName;

        [JsonProperty("queryPlanCost")]
        public float? QueryPlanCost { get; } = queryPlanCost;

        [JsonProperty("queryText")]
        public string QueryText { get; } = queryText;

        [JsonProperty("warnings")]
        public string Warnings { get; } = warnings;

        [JsonProperty("queryPlan")]
        public string QueryPlan { get; } = queryPlan;

        [JsonProperty("missingIndexes")]
        public string MissingIndexes { get; } = missingIndexes;

        [JsonProperty("implicitConversionInfo")]
        public string ImplicitConversionInfo { get; } = implicitConversionInfo;

        [JsonProperty("executionCount")]
        public long? ExecutionCount { get; } = executionCount;

        [JsonProperty("executionsPerMinute")]
        public decimal? ExecutionsPerMinute { get; } = executionsPerMinute;

        [JsonProperty("totalCPU")]
        public long? TotalCPU { get; } = totalCPU;

        [JsonProperty("averageCPU")]
        public long? AverageCPU { get; } = averageCPU;

        [JsonProperty("totalDuration")]
        public long? TotalDuration { get; } = totalDuration;

        [JsonProperty("averageDuration")]
        public long? AverageDuration { get; } = averageDuration;

        [JsonProperty("totalReads")]
        public long? TotalReads { get; } = totalReads;

        [JsonProperty("averageReads")]
        public long? AverageReads { get; } = averageReads;

        [JsonProperty("totalReturnedRows")]
        public long? TotalReturnedRows { get; } = totalReturnedRows;

        [JsonProperty("averageReturnedRows")]
        public decimal? AverageReturnedRows { get; } = averageReturnedRows;

        [JsonProperty("minReturnedRows")]
        public long? MinReturnedRows { get; } = minReturnedRows;

        [JsonProperty("maxReturnedRows")]
        public long? MaxReturnedRows { get; } = maxReturnedRows;

        [JsonProperty("numberOfPlans")]
        public int? NumberOfPlans { get; } = numberOfPlans;

        [JsonProperty("numberOfDistinctPlans")]
        public int? NumberOfDistinctPlans { get; } = numberOfDistinctPlans;

        [JsonProperty("lastExecutionTime")]
        public DateTime? LastExecutionTime { get; } = lastExecutionTime;

        [JsonProperty("queryHash")]
        public byte[]? QueryHash { get; } = queryHash;
    }
}