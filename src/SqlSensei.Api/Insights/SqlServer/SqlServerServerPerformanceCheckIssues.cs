using SqlSensei.Api.Storage;
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

    public class SqlServerPerformanceCheck(IEnumerable<SqlServerBadQuery> topBadQueries, string todayWaitType)
    {
        public IEnumerable<SqlServerBadQuery> TopBadQueries { get; } = topBadQueries;
        public string TodayWaitType { get; } = todayWaitType;
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
        public static readonly Dictionary<string, SqlServerPerformanceWaitType> waitTypeDictionary = new()
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
            {19, SqlServerPerformanceType.BatchRequestsPerSecond},
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

        public static readonly List<int> PerformanceCheckIds = [19, 26, 20, 23];

        public static IEnumerable<SqlServerPerformancePerformanceGraph> GetSqlServerPerformanceGraph(IEnumerable<MonitoringJobServerFindingLog> performanceLogs)
        {
            var hourlyPerformanceLogs = performanceLogs
                .GroupBy(x => new DateTime(x.Job.CreatedOn.Year, x.Job.CreatedOn.Month, x.Job.CreatedOn.Day, x.Job.CreatedOn.Hour, 0, 0))
                .ToList();

            var allPerformanceTypes = performanceDictionary.Values.Distinct().ToList();

            var result = new List<SqlServerPerformancePerformanceGraph>();

            foreach (var hourGroup in hourlyPerformanceLogs)
            {
                var performanceByType = hourGroup
                    .GroupBy(x => performanceDictionary.GetValueOrDefault(x.CheckId))
                    .ToDictionary(x => x.Key, x => x.Select(y => ParsePerformanceValue(y.Details, y.CheckId)).Average());

                foreach (var performanceType in allPerformanceTypes)
                {
                    var value = performanceByType.ContainsKey(performanceType) ? performanceByType[performanceType] : 0;
                    result.Add(new SqlServerPerformancePerformanceGraph(performanceType, value, hourGroup.Key));
                }
            }

            return result;
        }

        public static IEnumerable<SqlServerPerformanceWaitStatGraph> GetSqlServerWaitStatsGraph(IEnumerable<MonitoringJobServerWaitStatLog> waitStats)
        {
            var hourlyWaitStats = waitStats
                .Where(x => waitTypeDictionary.ContainsKey(x.Type))
                .GroupBy(x => new DateTime(x.Job.CreatedOn.Year, x.Job.CreatedOn.Month, x.Job.CreatedOn.Day, x.Job.CreatedOn.Hour, 0, 0))
                .ToList();

            var allWaitTypes = waitTypeDictionary.Values.Distinct().ToList();

            var result = new List<SqlServerPerformanceWaitStatGraph>();

            foreach (var hourGroup in hourlyWaitStats)
            {
                var statsByType = hourGroup
                    .GroupBy(x => waitTypeDictionary.GetValueOrDefault(x.Type))
                    .Select(x => new SqlServerPerformanceWaitStatGraph((long)Math.Round(x.Average(y => y.TimeInMs)), x.Key, hourGroup.Key));

                result.AddRange(statsByType);

                foreach (var waitType in allWaitTypes)
                {
                    if (!statsByType.Any(x => x.WaitType == waitType))
                    {
                        result.Add(new SqlServerPerformanceWaitStatGraph(0, waitType, hourGroup.Key));
                    }
                }
            }

            return result;
        }

        public static SqlServerPerformanceCheck GetSqlServerPerformanceFindings(IEnumerable<MonitoringQueryLog> badQueries, string topWaitType)
        {
            return new SqlServerPerformanceCheck(badQueries
                .Where(x => x is not null && x.QueryHash is not null)
                .GroupBy(x => x.QueryHash)
                .Select(x => x.Last())
                .OrderBy(x => x.TopNo)
                .Take(10)
                .Select(Convert), topWaitType);
        }

        public static string GetTopWaitType(IEnumerable<MonitoringJobServerWaitStatLog> waits)
        {
            var waitType = waits
                .GroupBy(x => waitTypeDictionary.GetValueOrDefault(x.Type))
                .Select(group => new { Category = group.Key, TotalAmount = group.Sum(g => g.TimeInMs) })
                .OrderByDescending(g => g.TotalAmount)
                .FirstOrDefault();

            return waitType?.Category switch
            {
                SqlServerPerformanceWaitType.CxPacket or SqlServerPerformanceWaitType.SosSchedulerYield or SqlServerPerformanceWaitType.Threadpool => "cpu",
                SqlServerPerformanceWaitType.Lock => "Duration",
                SqlServerPerformanceWaitType.ResourceSemaphore => "Memory grant",
                SqlServerPerformanceWaitType.PageIoLatch => "reads",
                SqlServerPerformanceWaitType.WriteLog => "writes",
                _ => "cpu",
            };
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

        private static double ParsePerformanceValue(string? value, int checkId)
        {
            if (performanceDictionary.TryGetValue(checkId, out var performanceType))
            {
                var match = Regex.Match(value ?? string.Empty, @"[\d\.]+");

                if (match.Success)
                {
                    return double.Parse(match.Value);
                }
            }

            return 0;
        }
    }
}
