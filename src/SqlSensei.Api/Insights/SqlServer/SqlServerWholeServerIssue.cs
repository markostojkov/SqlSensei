using SqlSensei.Api.Storage;

namespace SqlSensei.Api.Insights
{
    public class SqlServerWholeServerIssue
    {
        public static bool HasIssue(
            IEnumerable<MonitoringJobServerFindingLog> findings,
            IEnumerable<MonitoringJobServerLog> server,
            IEnumerable<MonitoringJobIndexMissingLog> missingIndex,
            IEnumerable<MonitoringJobIndexUsageLog> indexUsage)
        {
            var serverChecks = SqlServerServerCheckIssues.GetSqlServerChecks(server, findings);

            var index = SqlServerIndexIssues.GetSqlServerChecks(indexUsage, missingIndex);

            if (serverChecks.ServerIssues.Where(x => x.Priority <= 20).Any() ||
                serverChecks.CacheAndWaitStats.PoisonWaits ||
                serverChecks.CacheAndWaitStats.PoisonWaitsSerializableLocking ||
                serverChecks.CacheAndWaitStats.HighWait ||
                serverChecks.CacheAndWaitStats.LotOfCompilationsASec ||
                serverChecks.CacheAndWaitStats.LongRunningQueryBlockingOthers ||
                serverChecks.CacheAndWaitStats.LotOfReCompilationsASec)
            {
                return true;
            }

            return false;
        }
    }
}
