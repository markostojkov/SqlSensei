using Hangfire.States;

namespace SqlSensei.SqlServer.Hangfire
{
    public class SqlSenseiHangfireOptions
    {
        /// <summary>
        /// The cron expression defining the job's execution schedule. Default is "0 0 12 ? 1/1 SAT#4 *" (every First Saturday of the month).
        /// </summary>
        public string ExecuteMaintenanceJobCron { get; set; } = "0 0 * * 6#1";

        /// <summary>
        /// The cron expression defining the job's execution schedule. Default is "0 0 12 ? 1/1 SAT#4 *" (every First Saturday of the month).
        /// </summary>
        public string ExecuteMonitoringJobCron { get; set; } = "0 0 * * 6#1";

        /// <summary>
        /// The cron expression defining the job's execution schedule. Default is "0 */6 * * *" (every 6 hours).
        /// </summary>
        public string ExecuteMonitoringLogJobCron { get; set; } = "0 */6 * * *";

        public string Queue { get; set; } = EnqueuedState.DefaultQueue;

        public static SqlSenseiHangfireOptions Default => new SqlSenseiHangfireOptions { };
    }
}
