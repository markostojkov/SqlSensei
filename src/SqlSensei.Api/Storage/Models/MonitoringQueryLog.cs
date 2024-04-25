namespace SqlSensei.Api.Storage
{
    public class MonitoringQueryLog(
        long companyFk,
        long jobFk,
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
        long? totalCpu,
        long? averageCpu,
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
        public long Id { get; set; }
        public string WaitType { get; } = waitType;
        public int TopNo { get; } = topNo;
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string DatabaseName { get; set; } = databaseName;
        public float? QueryPlanCost { get; set; } = queryPlanCost;
        public string QueryText { get; set; } = queryText;
        public string Warnings { get; set; } = warnings;
        public string QueryPlan { get; set; } = queryPlan;
        public string MissingIndexes { get; set; } = missingIndexes;
        public string ImplicitConversionInfo { get; set; } = implicitConversionInfo;
        public long? ExecutionCount { get; set; } = executionCount;
        public decimal? ExecutionsPerMinute { get; set; } = executionsPerMinute;
        public long? TotalCpu { get; set; } = totalCpu;
        public long? AverageCpu { get; set; } = averageCpu;
        public long? TotalDuration { get; set; } = totalDuration;
        public long? AverageDuration { get; set; } = averageDuration;
        public long? TotalReads { get; set; } = totalReads;
        public long? AverageReads { get; set; } = averageReads;
        public long? TotalReturnedRows { get; set; } = totalReturnedRows;
        public decimal? AverageReturnedRows { get; set; } = averageReturnedRows;
        public long? MinReturnedRows { get; set; } = minReturnedRows;
        public long? MaxReturnedRows { get; set; } = maxReturnedRows;
        public int? NumberOfPlans { get; set; } = numberOfPlans;
        public int? NumberOfDistinctPlans { get; set; } = numberOfDistinctPlans;
        public DateTime? LastExecutionTime { get; set; } = lastExecutionTime;
        public byte[]? QueryHash { get; set; } = queryHash;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
