using SqlSensei.Api.Storage;
using System;
using System.Runtime.InteropServices;
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

    public enum SqlServerPerformanceType
    {
        CpuUtilization,
        WaitTimePerCorePerSec,
        ReCompilesPerSecond,
        BatchRequestsPerSecond
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

    public class SqlServerPerformanceWaitStatGraph(long timeInMs, SqlServerPerformanceWaitType waitType, DateTime? dateTime)
    {
        public long TimeInMs { get; } = timeInMs;
        public SqlServerPerformanceWaitType WaitType { get; } = waitType;
        public DateTime? DateTime { get; } = dateTime;
    }

    public class SqlServerPerformancePerformanceGraph(SqlServerPerformanceType type, double? value, DateTime? dateTime)
    {
        public SqlServerPerformanceType Type { get; } = type;
        public double? Value { get; } = value;
        public DateTime? DateTime { get; } = dateTime;
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

        private static readonly Dictionary<int, SqlServerPerformanceType> performanceDictionary = new()
        {
            {23, SqlServerPerformanceType.CpuUtilization},
            {10, SqlServerPerformanceType.BatchRequestsPerSecond},
            {26, SqlServerPerformanceType.ReCompilesPerSecond},
            {20, SqlServerPerformanceType.WaitTimePerCorePerSec},
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

        public static readonly List<int> PerformanceCheckIds = new() { 19, 26, 20, 23 };

        public static IEnumerable<SqlServerPerformancePerformanceGraph> GetSqlServerPerformanceGraph(IEnumerable<MonitoringJobServerFindingLog> performanceLogs)
        {
            return performanceLogs
                .GroupBy(x => new { CreatedOn = new DateTime(x.Job.CreatedOn.Year, x.Job.CreatedOn.Month, x.Job.CreatedOn.Day, x.Job.CreatedOn.Hour, 0, 0), Type = performanceDictionary.GetValueOrDefault(x.CheckId) })
                .Select(x => new SqlServerPerformancePerformanceGraph(
                    x.Key.Type,
                    x.Select(y =>
                    {
                        float val = 0;

                        if (x.Key.Type == SqlServerPerformanceType.CpuUtilization)
                        {
                            var percentageRegex = new Regex(@"(\d+)%");

                            var percentageMatch = percentageRegex.Match(y.Details);

                            if (percentageMatch.Success)
                            {
                                val = float.Parse(percentageMatch.Groups[1].Value);
                            }
                        }
                        else
                        {
                            if (float.TryParse(y.Details, out var x))
                            {
                                val = x;
                            }
                        }

                        return val;
                    }).Average(y => y),
                    x.Key.CreatedOn));
        }

        public static IEnumerable<SqlServerPerformanceWaitStatGraph> GetSqlServerWaitStatsGraph(IEnumerable<MonitoringJobServerWaitStatLog> waitStats)
        {
            return waitStats
                .Where(x => waitTypeDictionary.ContainsKey(x.Type))
                .GroupBy(x => new { CreatedOn = new DateTime(x.Job.CreatedOn.Year, x.Job.CreatedOn.Month, x.Job.CreatedOn.Day, x.Job.CreatedOn.Hour, 0, 0), WaitType = waitTypeDictionary.GetValueOrDefault(x.Type) })
                .Select(x => new SqlServerPerformanceWaitStatGraph(
                    (long)Math.Round(x.Average(y => y.TimeInMs)),
                    x.Key.WaitType,
                    x.Key.CreatedOn));
        }

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
                reCompilesPerSecond = y;
            }

            if (float.TryParse(logs.FirstOrDefault(x => x.CheckId == 20)?.Details, out var z))
            {
                waitTimePerCorePerSec = z;
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

            return new SqlServerPerformanceCheck(serverInfo, waitStats, badQueries.Select(x => Convert(x)));
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
