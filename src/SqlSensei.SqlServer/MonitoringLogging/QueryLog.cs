#nullable enable

using Microsoft.Data.SqlClient;
using SqlSensei.Core;
using System;
using System.Collections.Generic;

namespace SqlSensei.SqlServer.InformationGather
{
    public class QueryLog(
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
        byte[]? queryHash) : IMonitoringJobQueryLog
    {
        public string DatabaseName { get; } = databaseName;
        public float? QueryPlanCost { get; } = queryPlanCost;
        public string QueryText { get; } = queryText;
        public string Warnings { get; } = warnings;
        public string QueryPlan { get; } = queryPlan;
        public string MissingIndexes { get; } = missingIndexes;
        public string ImplicitConversionInfo { get; } = implicitConversionInfo;
        public long? ExecutionCount { get; } = executionCount;
        public decimal? ExecutionsPerMinute { get; } = executionsPerMinute;
        public long? TotalCPU { get; } = totalCPU;
        public long? AverageCPU { get; } = averageCPU;
        public long? TotalDuration { get; } = totalDuration;
        public long? AverageDuration { get; } = averageDuration;
        public long? TotalReads { get; } = totalReads;
        public long? AverageReads { get; } = averageReads;
        public long? TotalReturnedRows { get; } = totalReturnedRows;
        public decimal? AverageReturnedRows { get; } = averageReturnedRows;
        public long? MinReturnedRows { get; } = minReturnedRows;
        public long? MaxReturnedRows { get; } = maxReturnedRows;
        public int? NumberOfPlans { get; } = numberOfPlans;
        public int? NumberOfDistinctPlans { get; } = numberOfDistinctPlans;
        public DateTime? LastExecutionTime { get; } = lastExecutionTime;
        public byte[]? QueryHash { get; } = queryHash;

        public static List<QueryLog> GetAll(SqlDataReader reader)
        {
            var records = new List<QueryLog>();

            while (reader.Read())
            {
                var record = new QueryLog(
                    reader.GetString(reader.GetOrdinal("DatabaseName")),
                    reader.IsDBNull(reader.GetOrdinal("QueryPlanCost")) ? null : (float?)reader.GetDouble(reader.GetOrdinal("QueryPlanCost")),
                    reader.IsDBNull(reader.GetOrdinal("QueryText")) ? string.Empty : reader.GetString(reader.GetOrdinal("QueryText")),
                    reader.IsDBNull(reader.GetOrdinal("Warnings")) ? string.Empty : reader.GetString(reader.GetOrdinal("Warnings")),
                    reader.IsDBNull(reader.GetOrdinal("QueryPlan")) ? string.Empty : reader.GetString(reader.GetOrdinal("QueryPlan")),
                    string.Empty,
                    string.Empty,
                    reader.IsDBNull(reader.GetOrdinal("ExecutionCount")) ? null : reader.GetInt64(reader.GetOrdinal("ExecutionCount")),
                    reader.IsDBNull(reader.GetOrdinal("ExecutionsPerMinute")) ? null : reader.GetDecimal(reader.GetOrdinal("ExecutionsPerMinute")),
                    reader.IsDBNull(reader.GetOrdinal("TotalCPU")) ? null : reader.GetInt64(reader.GetOrdinal("TotalCPU")),
                    reader.IsDBNull(reader.GetOrdinal("AverageCPU")) ? null : reader.GetInt64(reader.GetOrdinal("AverageCPU")),
                    reader.IsDBNull(reader.GetOrdinal("TotalDuration")) ? null : reader.GetInt64(reader.GetOrdinal("TotalDuration")),
                    reader.IsDBNull(reader.GetOrdinal("AverageDuration")) ? null : reader.GetInt64(reader.GetOrdinal("AverageDuration")),
                    reader.IsDBNull(reader.GetOrdinal("TotalReads")) ? null : reader.GetInt64(reader.GetOrdinal("TotalReads")),
                    reader.IsDBNull(reader.GetOrdinal("AverageReads")) ? null : reader.GetInt64(reader.GetOrdinal("AverageReads")),
                    reader.IsDBNull(reader.GetOrdinal("TotalReturnedRows")) ? null : reader.GetInt64(reader.GetOrdinal("TotalReturnedRows")),
                    reader.IsDBNull(reader.GetOrdinal("AverageReturnedRows")) ? null : reader.GetDecimal(reader.GetOrdinal("AverageReturnedRows")),
                    reader.IsDBNull(reader.GetOrdinal("MinReturnedRows")) ? null : reader.GetInt64(reader.GetOrdinal("MinReturnedRows")),
                    reader.IsDBNull(reader.GetOrdinal("MaxReturnedRows")) ? null : reader.GetInt64(reader.GetOrdinal("MaxReturnedRows")),
                    reader.IsDBNull(reader.GetOrdinal("NumberOfPlans")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfPlans")),
                    reader.IsDBNull(reader.GetOrdinal("NumberOfDistinctPlans")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfDistinctPlans")),
                    reader.IsDBNull(reader.GetOrdinal("LastExecutionTime")) ? null : reader.GetDateTime(reader.GetOrdinal("LastExecutionTime")),
                    reader.IsDBNull(reader.GetOrdinal("QueryHash")) ? null : reader.GetSqlBinary(reader.GetOrdinal("QueryHash")).Value
                );

                records.Add(record);
            }

            return records;
        }
    }
}