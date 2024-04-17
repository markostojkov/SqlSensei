using SqlSensei.Api.Storage;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SqlSensei.Api.Insights
{
    public enum SqlServerPerformanceWaitType
    {
        CxPacket,
        SosSchedulerYield,
        Threadpool,
        Lock,
        ResourceSemaphore,
        AsyncNetworkIo,
        PageIoLatch,
        WriteLog
    }

    public class SqlServerBadQuery(
        long id,
        string? databaseName,
        long? executionsCount,
        decimal? executionsPerMinuteCount,
        long? totalCpu,
        long? averageCpu,
        long? totalDuration,
        long? averageDuration,
        long? totalReads,
        long? averageReads,
        long? totalReturnedRows,
        decimal? averageReturnedRows,
        int? numberOfPlans,
        int? numberOfDistinctPlans,
        DateTime? lastExecutionTime,
        byte[]? queryHash)
    {
        public long Id { get; } = id;
        public string? DatabaseName { get; } = databaseName;
        public long? ExecutionsCount { get; } = executionsCount;
        public decimal? ExecutionsPerMinuteCount { get; } = executionsPerMinuteCount;
        public long? TotalCpu { get; } = totalCpu;
        public long? AverageCpu { get; } = averageCpu;
        public long? TotalDuration { get; } = totalDuration;
        public long? AverageDuration { get; } = averageDuration;
        public long? TotalReads { get; } = totalReads;
        public long? AverageReads { get; } = averageReads;
        public long? TotalReturnedRows { get; } = totalReturnedRows;
        public decimal? AverageReturnedRows { get; } = averageReturnedRows;
        public int? NumberOfPlans { get; } = numberOfPlans;
        public int? NumberOfDistinctPlans { get; } = numberOfDistinctPlans;
        public DateTime? LastExecutionTime { get; } = lastExecutionTime;
        public byte[]? QueryHash { get; } = queryHash;
    }

    public class SqlServerPerformanceCheck(
        SqlServerPerformanceServerInfo serverInfo,
        IEnumerable<SqlServerPerformanceWaitStat> waitStats,
        IEnumerable<SqlServerBadQuery> topBadQueries)
    {
        public SqlServerPerformanceServerInfo ServerInfo { get; } = serverInfo;
        public IEnumerable<SqlServerPerformanceWaitStat> WaitStats { get; } = waitStats;
        public IEnumerable<SqlServerBadQuery> TopBadQueries { get; } = topBadQueries;
    }

    public class SqlServerPerformanceServerInfo(float? batchRequestsPerSecond, float? reCompilesPerSecond, float? waitTimePerCorePerSec, float? cpuUtilization)
    {
        public float? BatchRequestsPerSecond { get; } = batchRequestsPerSecond;
        public float? ReCompilesPerSecond { get; } = reCompilesPerSecond;
        public float? WaitTimePerCorePerSec { get; } = waitTimePerCorePerSec;
        public float? CpuUtilization { get; } = cpuUtilization;
    }

    public class SqlServerPerformanceWaitStat(long timeInMs, SqlServerPerformanceWaitType waitType)
    {
        public long TimeInMs { get; } = timeInMs;
        public SqlServerPerformanceWaitType WaitType { get; } = waitType;
    }

    public class SqlServerServerPerformanceCheckIssues
    {
        private static readonly Dictionary<string, SqlServerPerformanceWaitType> waitTypeDictionary = new()
        {
            {"ASYNC_NETWORK_IO", SqlServerPerformanceWaitType.AsyncNetworkIo},
            {"CXCONSUMER", SqlServerPerformanceWaitType.CxPacket},
            {"CXPACKET", SqlServerPerformanceWaitType.CxPacket},
            {"LCK_M_S", SqlServerPerformanceWaitType.Lock},
            {"LCK_M_X", SqlServerPerformanceWaitType.Lock},
            {"PAGEIOLATCH_EX", SqlServerPerformanceWaitType.PageIoLatch},
            {"PAGEIOLATCH_SH", SqlServerPerformanceWaitType.PageIoLatch},
            {"PAGEIOLATCH_UP", SqlServerPerformanceWaitType.PageIoLatch},
            {"SOS_SCHEDULER_YIELD", SqlServerPerformanceWaitType.SosSchedulerYield},
            {"THREADPOOL", SqlServerPerformanceWaitType.Threadpool},
            {"WRITELOG", SqlServerPerformanceWaitType.WriteLog}
        };

        private static readonly Dictionary<int, string> queryIssues = new()
        {
            {5, "Long-Running Query Blocking Others"},
            {9, "Query Rolling Back"},
            {8, "Sleeping Query with Open Transactions"},
            {15, "Compilations/Sec High"},
            {16, "Re-Compilations/Sec High"},
            {44, "Statistics Updated Recently"},
            {47, "High Percentage Of Runnable Queries"},
            {29, "Forwarded Fetches/Sec High"},
        };

        private static readonly Dictionary<int, string> serverPerformance = new()
        {
            {35, "Target Memory Lower Than Max"},
            {24, "High CPU Utilization"},
            {28, "High CPU Utilization - Non SQL Processes"},
            {11, "Slow Data File Reads"},
            {12, "Slow Log File Writes"},
            {39, "Memory Grants pending"},
        };

        private static readonly Dictionary<int, string> serverInfo = new()
        {
            {19, "Batch Requests per Second"},
            {26, "Re-Compiles per Second"},
            {20, "Wait Time per Core per Second"},
            {23, "CPU Utilization"},
        };

        public static SqlServerPerformanceCheck GetSqlServerPerformanceFindings(
            IEnumerable<MonitoringJobServerFindingLog> logs,
            IEnumerable<MonitoringJobServerWaitStatLog> logsWaitStats,
            IEnumerable<MonitoringQueryLog> badQueries)
        {
            var cpuLog = logs.FirstOrDefault(x => x.CheckId == 23);
            float? batchRequestsPerSecond = null;
            float? reCompilesPerSecond = null;
            float? waitTimePerCorePerSec = null;
            float? cpuUtilization = null;

            if (float.TryParse(logs.FirstOrDefault(x => x.CheckId == 19)?.Details, out var x))
            {
                batchRequestsPerSecond = x;
            }

            if (float.TryParse(logs.FirstOrDefault(x => x.CheckId == 26)?.Details, out var y))
            {
                reCompilesPerSecond = x;
            }

            if (float.TryParse(logs.FirstOrDefault(x => x.CheckId == 20)?.Details, out var z))
            {
                waitTimePerCorePerSec = x;
            }

            if (cpuLog != null)
            {
                var percentageRegex = new Regex(@"(\d+)%");

                var percentageMatch = percentageRegex.Match(cpuLog.Details);

                if (percentageMatch.Success)
                {
                    cpuUtilization = float.Parse(percentageMatch.Groups[1].Value);
                }
            }

            var serverInfo = new SqlServerPerformanceServerInfo(batchRequestsPerSecond, reCompilesPerSecond, waitTimePerCorePerSec, cpuUtilization);

            var waitStats = logsWaitStats
                .Where(x => waitTypeDictionary.ContainsKey(x.Type))
                .Select(x => new SqlServerPerformanceWaitStat(x.TimeInMs, waitTypeDictionary.GetValueOrDefault(x.Type)))
                .OrderByDescending(stats => stats.TimeInMs);

            var selectedBadQueriesByWaitType = new List<SqlServerBadQuery>();

            if (waitStats.Any())
            {
                switch (waitStats.First().WaitType)
                {
                    case SqlServerPerformanceWaitType.CxPacket:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Cpu).Select(x => Convert(x)));
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Reads).Select(x => Convert(x)));
                        break;
                    case SqlServerPerformanceWaitType.SosSchedulerYield:
                    case SqlServerPerformanceWaitType.Threadpool:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Reads).Select(x => Convert(x)));
                        break;
                    case SqlServerPerformanceWaitType.Lock:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Duration).Select(x => Convert(x)));
                        break;
                    case SqlServerPerformanceWaitType.ResourceSemaphore:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.MemoryGrant).Select(x => Convert(x)));
                        break;
                    case SqlServerPerformanceWaitType.PageIoLatch:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Reads).Select(x => Convert(x)));
                        break;
                    case SqlServerPerformanceWaitType.WriteLog:
                        selectedBadQueriesByWaitType.AddRange(badQueries.Where(x => x.QueryLogSortBy == QueryLogSortBy.Writes).Select(x => Convert(x)));
                        break;
                }
            }

            return new SqlServerPerformanceCheck(serverInfo, waitStats, selectedBadQueriesByWaitType);
        }

        private static SqlServerBadQuery Convert(MonitoringQueryLog x)
        {
            return new SqlServerBadQuery(
                x.Id,
                x.DatabaseName,
                x.ExecutionCount,
                x.ExecutionsPerMinute,
                x.TotalCpu,
                x.AverageCpu,
                x.TotalDuration,
                x.AverageDuration,
                x.TotalReads,
                x.AverageReads,
                x.TotalReturnedRows,
                x.AverageReturnedRows,
                x.NumberOfPlans,
                x.NumberOfDistinctPlans,
                x.LastExecutionTime,
                x.QueryHash);
        }
    }
}
