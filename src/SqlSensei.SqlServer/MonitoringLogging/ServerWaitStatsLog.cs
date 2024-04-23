using SqlSensei.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlSensei.SqlServer.InformationGather
{
    public class ServerWaitStatsLog(string type, long timeInMs, string waitCategory) : IMonitoringJobServerPerformanceLogWaitStat
    {
        public string Type { get; } = type;
        public long TimeInMs { get; } = timeInMs;
        public string WaitCategory { get; } = waitCategory;

        public static List<ServerWaitStatsLog> GetAll(DataTable dataTable)
        {
            var records = new List<ServerWaitStatsLog>();

            foreach (DataRow row in dataTable.Rows)
            {
                var type = row.IsNull("wait_type") ? string.Empty : Convert.ToString(row["wait_type"]);
                var timeInMs = row.IsNull("Wait Time (Seconds)") ? -1 : Convert.ToInt64(row["Wait Time (Seconds)"]);
                var waitCategory = row.IsNull("wait_category") ? string.Empty : Convert.ToString(row["wait_category"]);

                timeInMs *= 1000;

                var record = new ServerWaitStatsLog(type, timeInMs, waitCategory);

                records.Add(record);
            }

            return records;
        }

        public static string WaitType(List<ServerWaitStatsLog> logs)
        {
            var topWait = logs.OrderByDescending(x => x.TimeInMs).FirstOrDefault();
            var topWaitType = "cpu";

            if (topWait != null)
            {
                if (topWait.WaitCategory == "Lock")
                {
                    topWaitType = "Duration";
                }

                if (topWait.WaitCategory == "Memory")
                {
                    topWaitType = "Memory grant";
                }

                if (topWait.WaitCategory == "Buffer IO")
                {
                    topWaitType = "reads";
                }

                if (topWait.WaitCategory == "Tran Log IO")
                {
                    topWaitType = "writes";
                }
            }

            return topWaitType;
        }
    }
}