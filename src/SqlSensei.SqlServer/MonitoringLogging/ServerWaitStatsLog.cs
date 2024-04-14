using Microsoft.Data.SqlClient;

using SqlSensei.Core;
using System.Collections.Generic;

namespace SqlSensei.SqlServer.InformationGather
{
    public class ServerWaitStatsLog(string type, long timeInMs) : IMonitoringJobServerPerformanceLogWaitStat
    {
        public string Type { get; } = type;
        public long TimeInMs { get; } = timeInMs;

        public static List<ServerWaitStatsLog> GetAll(SqlDataReader reader)
        {
            List<ServerWaitStatsLog> records = [];

            while (reader.Read())
            {
                var type = reader.IsDBNull(reader.GetOrdinal("wait_type")) ? string.Empty : reader.GetString(reader.GetOrdinal("wait_type"));
                var timeInMs = reader.IsDBNull(reader.GetOrdinal("wait_type_ms")) ? -1 : reader.GetInt64(reader.GetOrdinal("wait_type_ms"));

                var record = new ServerWaitStatsLog(type, timeInMs);

                records.Add(record);
            }

            return records;
        }
    }
}