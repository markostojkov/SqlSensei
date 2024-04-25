using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SqlSensei.Api.Storage;
using System.Text.RegularExpressions;

namespace SqlSensei.Api.Insights
{
    public enum ServerCheckIssueCategory
    {
        DataInDanger,
        ServerInDanger,
        ServerPerformance,
        ServerCacheClearAndWaits,
        ServerInfo,
        None
    }

    public class SqlServerCheck(
        SqlServerInsightsCacheAndWaitStats cacheAndWaitStats,
        IEnumerable<SqlServerInsightsServerIssue> serverIssues)
    {
        public SqlServerInsightsCacheAndWaitStats CacheAndWaitStats { get; } = cacheAndWaitStats;
        public IEnumerable<SqlServerInsightsServerIssue> ServerIssues { get; } = serverIssues;
    }

    public class SqlServerInsightsServerIssue(short priority, int checkId, ServerCheckIssueCategory checkCategory, string details, string scriptDetails, string? databaseName)
    {
        public short Priority { get; } = priority;
        public int CheckId { get; } = checkId;
        public ServerCheckIssueCategory CheckCategory { get; } = checkCategory;
        public string Details { get; } = details;
        public string ScriptDetails { get; } = scriptDetails;
        public string? DatabaseName { get; } = databaseName;
    }

    public class SqlServerInsightsCacheAndWaitStats(
        bool waitsClearedRecently,
        bool cacheClearedRecently,
        bool noSignificantWaitStats,
        bool poisonWaits,
        string? poisonWaitType,
        bool poisonWaitsSerializableLocking,
        bool longRunningQueryBlockingOthers,
        string? longRunningQueryBlockingOthersDetails,
        bool lotOfForwardedFetchesExist,
        bool lotOfCompilationsASec,
        bool lotOfReCompilationsASec,
        bool statisticsUpdatedRecently
        )
    {
        public bool WaitsClearedRecently { get; } = waitsClearedRecently;
        public bool CacheClearedRecently { get; } = cacheClearedRecently;
        public bool NoSignificantWaitStats { get; } = noSignificantWaitStats;
        public bool PoisonWaits { get; } = poisonWaits;
        public string? PoisonWaitType { get; } = poisonWaitType;
        public bool PoisonWaitsSerializableLocking { get; } = poisonWaitsSerializableLocking;
        public bool LongRunningQueryBlockingOthers { get; } = longRunningQueryBlockingOthers;
        public string? LongRunningQueryBlockingOthersDetails { get; } = longRunningQueryBlockingOthersDetails;
        public bool LotOfForwardedFetchesExist { get; } = lotOfForwardedFetchesExist;
        public bool LotOfCompilationsASec { get; } = lotOfCompilationsASec;
        public bool LotOfReCompilationsASec { get; } = lotOfReCompilationsASec;
        public bool StatisticsUpdatedRecently { get; } = statisticsUpdatedRecently;
    }

    public class SqlServerInsightsServerInfo(
        bool is32Bit,
        DateTime? lastRestartServer,
        DateTime? lastRestartSqlServer,
        int? databaseCount,
        float? databaseSizeInGb,
        int? defaultTraceContentInHours,
        int? logicalCpu,
        int? memoryInGb,
        string? serverName,
        string? versionDetails,
        string? edition)
    {
        public bool Is32Bit { get; } = is32Bit;
        public DateTime? LastRestartServer { get; } = lastRestartServer;
        public DateTime? LastRestartSqlServer { get; } = lastRestartSqlServer;
        public int? DatabaseCount { get; } = databaseCount;
        public float? DatabaseSizeInGb { get; } = databaseSizeInGb;
        public int? DefaultTraceContentInHours { get; } = defaultTraceContentInHours;
        public int? LogicalCpu { get; } = logicalCpu;
        public int? MemoryInGb { get; } = memoryInGb;
        public string? ServerName { get; } = serverName;
        public string? VersionDetails { get; } = versionDetails;
        public string? Edition { get; } = edition;
    }

    public class SqlServerServerCheckIssues
    {
        private static readonly Dictionary<int, string> dataInDanger = new()
        {
            {93,  "Backup is on the same storage as the database"},
            {1,   "Backup not performed recently"},
            {202, "Encryption Certificate Not Backed Up Recently"},
            {119, "TDE Certificate Not Backed Up Recently"},
            {2,   "Full Recovery Mode without Log Backups"},
            {256, "Log Backups to NUL"},
            {34,  "Database Corruption Detected"},
            {89,  "Database Corruption Detected"},
            {90,  "Database Corruption Detected"},
            {68,  "Last good DBCC CHECKDB over 2 weeks old"},
            {129, "Dangerous Build of SQL Server (Corruption)"},
            {102, "Databases in Unusual States"},
            {14,  "Page Verification Not Optimal"}
        };

        private static readonly Dictionary<int, string> serverInDanger = new()
        {
            {51,  "Memory Dangerously Low"},
            {101, "CPU Schedulers Offline"},
            {198, "CPU with Odd Number of Cores"},
            {110, "Memory Nodes Offline"},
            {209, "DBCC WRITEPAGE Used Recently"},
            {157, "Dangerous Build of SQL Server (Security)"},
            {184, "No Failover Cluster Nodes Available"},
            {128, "Unsupported Build of SQL Server"}
        };

        private static readonly Dictionary<int, string> serverPerformance = new()
        {
            {12,  "Auto-Close Enabled"},
            {13,  "Auto-Shrink Enabled"},
            {206, "Auto-Shrink Ran Recently"},
            {182, "Query Store Cleanup Disabled"},
            {26,  "User Databases on C Drive"},
            {25,  "TempDB on C Drive"},
            {151, "File Growths Slow"},
            {192, "Instant File Initialization Not Enabled"},
            {3,   "MSDB Backup History Not Purged"},
            {186, "MSDB Backup History Purged Too Frequently"},
            {257, "Recovery Interval Not Optimal"},
            {236, "Snapshotting Too Many Databases"},
            {165, "Too Much Free Memory"},
            {113, "Full Text Indexes Not Updating"},
            {191, "TempDB File Error"},
            {75,  "Transaction Log Larger than Data File"},
            {60,  "Fill Factor Changed"},
            {161, "High Number of Cached Plans"},
            {47,  "Indexes Disabled"},
            {160, "Many Plans for One Query"},
            {50,  "Max Memory Set Too High"},
            {117, "Memory Pressure Affecting Queries"},
            {79,  "Shrink Database Job"},
            {180, "Shrink Database Step In Maintenance Plan"},
            {35,  "Single-Use Plans in Procedure Cache"},
            {78,  "Stored Procedure WITH RECOMPILE"},
            {215, "Implicit Transactions"},
            {38,  "Active Tables Without Clustered Indexes"},
            {15,  "Auto-Create Stats Disabled"},
            {16,  "Auto-Update Stats Disabled"},
            {220, "Statistics Without Histograms"},
            {66,  "Cursor"},
            {63,  "Implicit Conversion"},
            {64,  "Implicit Conversion Affecting Cardinality"},
            {65,  "Missing Index"},
            {118, "RID or Key Lookups"},
            {67,  "Scalar UDFs"},
            {124, "Deadlocks Happening Daily"},
            {46,  "Leftover Fake Indexes From Wizards"},
            {218, "Objects created with dangerous SET Options"},
            {45,  "Queries Forcing Join Hints"},
            {44,  "Queries Forcing Order Hints"},
            {36,  "Slow Storage Reads on Drive <DRIVELETTER>"},
            {37,  "Slow Storage Writes on Drive <DRIVELETTER>"},
            {17,  "Stats Updated Asynchronously"},
            {32,  "Triggers on Tables"},
            {158, "File growth set to 1MB"},
            {82,  "File growth set to percent"},
            {69,  "High VLF Count"},
            {41,  "Multiple Log Files on One Drive"},
            {24,  "System Database on C Drive"},
            {175, "TempDB Has >16 Data Files"},
            {40,  "TempDB Only Has 1 Data File"},
            {183, "TempDB Unevenly Sized Data Files"},
            {42,  "Uneven File Growth Settings in One Filegroup"},
            {210, "DBCC SHRINK% Ran Recently"},
        };

        private static readonly Dictionary<int, string> serverCacheClearAndWaits = new()
        {
            {207, "DBCC DROPCLEANBUFFERS Ran Recently"},
            {208, "DBCC FREEPROCCACHE Ran Recently"},
            {125, "Plan Cache Erased Recently"},
            {221, "Server restarted in last 24 hours"},
            {205, "Wait Stats Cleared Recently"},
            {185, "Wait Stats Have Been Cleared"},
            {153, "No Significant Waits Detected"},
            {107, "Poison Wait Detected"},
            {121, "Poison Wait Detected: Serializable Locking"}
        };

        private static readonly Dictionary<int, string> serverInfo = new()
        {
            {232,   "Data Size"},
            {106,   "Default Trace Contents"},
            {92,    "Drive Space"},
            {84,    "Hardware"},
            {130,   "Server Name"},
            {88,    "SQL Server Last Restart"},
            {91,    "Server Last Restart"},
            {85,    "SQL Server Service"},
            {154,   "32-bit SQL Server Installed"},
        };

        public static SqlServerCheck GetSqlServerChecks(IEnumerable<MonitoringJobServerLog> logs)
        {
            var serverIssues = logs
                .Where(x => GetCategory(x.CheckId) != ServerCheckIssueCategory.None)
                .Select(x => new SqlServerInsightsServerIssue(x.Priority, x.CheckId, GetCategory(x.CheckId), GetInsightsDetails(x), x.Details, x.DatabaseName))
                .GroupBy(x => new { x.CheckId, x.DatabaseName })
                .Select(x => x.First());

            var waitStatsClearedRecently = logs.Any(x => new List<int>() { 221, 205, 185 }.Contains(x.CheckId));
            var cacheClearedRecently = logs.Any(x => new List<int>() { 207, 208, 125, 221 }.Contains(x.CheckId));
            var noSignificantWaitStats = logs.Any(x => x.CheckId == 153);
            var poisonWaits = logs.FirstOrDefault(x => x.CheckId == 107);
            var poisonWaitsSerializableLocking = logs.Any(x => x.CheckId == 121);
            var longRunningQueryBlockingOthers = logs.FirstOrDefault(x => x.CheckId == 5);
            var lotOfForwardedFetchesExist = logs.Any(x => x.CheckId == 29);
            var lotOfCompilationsASec = logs.Any(x => x.CheckId == 15);
            var lotOfReCompilationsASec = logs.Any(x => x.CheckId == 16);
            var statisticsUpdatedRecently = logs.Any(x => x.CheckId == 44);

            var cacheAndWaitStats = new SqlServerInsightsCacheAndWaitStats(
                waitStatsClearedRecently,
                cacheClearedRecently,
                noSignificantWaitStats,
                poisonWaits != null,
                poisonWaits?.Details,
                poisonWaitsSerializableLocking,
                longRunningQueryBlockingOthers != null,
                longRunningQueryBlockingOthers?.Details,
                lotOfForwardedFetchesExist,
                lotOfCompilationsASec,
                lotOfReCompilationsASec,
                statisticsUpdatedRecently);

            return new SqlServerCheck(cacheAndWaitStats, serverIssues);
        }

        public static SqlServerInsightsServerInfo GetServerInfo(IEnumerable<MonitoringJobServerLog> logs)
        {
            var serverIssues = logs
                .Where(x => GetCategory(x.CheckId) != ServerCheckIssueCategory.None)
                .Select(x => new SqlServerInsightsServerIssue(x.Priority, x.CheckId, GetCategory(x.CheckId), GetInsightsDetails(x), x.Details, x.DatabaseName));

            var is32Bit = logs.Any(x => x.CheckId == 154);
            var serverName = logs.FirstOrDefault(x => x.CheckId == 130)?.Details;
            var versionDetails = logs.FirstOrDefault(x => x.CheckId == 85)?.Details;
            var databaseInfo = logs.FirstOrDefault(x => x.CheckId == 232);
            var defaultTrace = logs.FirstOrDefault(x => x.CheckId == 106);
            var hardware = logs.FirstOrDefault(x => x.CheckId == 84);
            DateTime? lastServerRestart = null;
            DateTime? lastSqlServerRestart = null;
            int? databaseCount = null;
            float? databaseSizeInGb = null;
            int? defaultTraceContentInHours = null;
            int? logicalCpus = null;
            int? memoryInGb = null;
            string? versionEdition = null;

            if (DateTime.TryParse(logs.FirstOrDefault(x => x.CheckId == 91)?.Details, out var x))
            {
                lastServerRestart = x;
            }

            if (DateTime.TryParse(logs.FirstOrDefault(x => x.CheckId == 88)?.Details, out var y))
            {
                lastSqlServerRestart = y;
            }

            if (databaseInfo != null)
            {
                var dbRegex = new Regex(@"(\d+) databases");
                var sizeRegex = new Regex(@"([\d.]+) GB");

                var dbMatch = dbRegex.Match(databaseInfo.Details);
                if (dbMatch.Success)
                {
                    databaseCount = int.Parse(dbMatch.Groups[1].Value);
                }

                var sizeMatch = sizeRegex.Match(databaseInfo.Details);
                if (sizeMatch.Success)
                {
                    databaseSizeInGb = float.Parse(sizeMatch.Groups[1].Value);
                }
            }

            if (defaultTrace != null)
            {
                var hoursRegex = new Regex(@"(\d+) hours");

                var hoursMatch = hoursRegex.Match(defaultTrace.Details);
                if (hoursMatch.Success)
                {
                    defaultTraceContentInHours = int.Parse(hoursMatch.Groups[1].Value);
                }
            }

            if (hardware != null)
            {
                var logicalProcessorsRegex = new Regex(@"Logical processors: (\d+)");
                var physicalMemoryRegex = new Regex(@"Physical memory: (\d+(?:\.\d+)?)GB");

                var logicalProcessorsMatch = logicalProcessorsRegex.Match(hardware.Details);
                if (logicalProcessorsMatch.Success)
                {
                    logicalCpus = int.Parse(logicalProcessorsMatch.Groups[1].Value);
                }

                var physicalMemoryMatch = physicalMemoryRegex.Match(hardware.Details);
                if (physicalMemoryMatch.Success)
                {
                    memoryInGb = int.Parse(physicalMemoryMatch.Groups[1].Value);
                }
            }

            if (versionDetails != null)
            {
                var regex = new Regex(@"Edition: (\w+ \w+)");

                var match = regex.Match(versionDetails);

                if (match.Success)
                {
                    versionEdition = match.Groups[1].Value;
                }
            }

            return new SqlServerInsightsServerInfo(
                is32Bit,
                lastServerRestart,
                lastSqlServerRestart,
                databaseCount,
                databaseSizeInGb,
                defaultTraceContentInHours,
                logicalCpus,
                memoryInGb,
                serverName,
                versionDetails,
                versionEdition);
        }

        private static ServerCheckIssueCategory GetCategory(int checkId)
        {
            if (dataInDanger.ContainsKey(checkId))
            {
                return ServerCheckIssueCategory.DataInDanger;
            }

            if (serverInDanger.ContainsKey(checkId))
            {
                return ServerCheckIssueCategory.ServerInDanger;
            }

            if (serverPerformance.ContainsKey(checkId))
            {
                return ServerCheckIssueCategory.ServerPerformance;
            }

            return ServerCheckIssueCategory.None;
        }

        private static string GetInsightsDetails(MonitoringJobServerLog log)
        {
            if (dataInDanger.TryGetValue(log.CheckId, out var x))
            {
                return x;
            }

            if (serverInDanger.TryGetValue(log.CheckId, out var y))
            {
                return y;
            }

            if (serverPerformance.TryGetValue(log.CheckId, out var z))
            {
                return z;
            }

            return string.Empty;
        }
    }
}
