#nullable enable

using System;

namespace SqlSensei.Core
{
    public interface IMonitoringJobQueryLog : IDatabaseLog
    {
        public string WaitType { get; }
        public int TopNo { get; }
        public float? QueryPlanCost { get; }
        public string QueryText { get; }
        public string Warnings { get; }
        public string QueryPlan { get; }
        public string MissingIndexes { get; }
        public string ImplicitConversionInfo { get; }
        public long? ExecutionCount { get; }
        public decimal? ExecutionsPerMinute { get; }
        public long? TotalCPU { get; }
        public long? AverageCPU { get; }
        public long? TotalDuration { get; }
        public long? AverageDuration { get; }
        public long? TotalReads { get; }
        public long? AverageReads { get; }
        public long? TotalReturnedRows { get; }
        public decimal? AverageReturnedRows { get; }
        public long? MinReturnedRows { get; }
        public long? MaxReturnedRows { get; }
        public int? NumberOfPlans { get; }
        public int? NumberOfDistinctPlans { get; }
        public DateTime? LastExecutionTime { get; }
        public byte[]? QueryHash { get; }
    }
}