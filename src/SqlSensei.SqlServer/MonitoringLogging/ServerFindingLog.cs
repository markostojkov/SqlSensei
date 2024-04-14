using Microsoft.Data.SqlClient;

using SqlSensei.Core;
using System.Collections.Generic;

namespace SqlSensei.SqlServer.InformationGather
{
    public class ServerFindingLog(int checkId, short priority, string details) : IMonitoringJobServerPerformanceLogFinding
    {
        public int CheckId { get; } = checkId;
        public short Priority { get; } = priority;
        public string Details { get; } = details;

        public static List<ServerFindingLog> GetAll(SqlDataReader reader)
        {
            List<ServerFindingLog> records = [];

            while (reader.Read())
            {
                var checkId = reader.IsDBNull(reader.GetOrdinal("CheckID")) ? -1 : reader.GetInt32(reader.GetOrdinal("CheckID"));
                var priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? (short)-1 : reader.GetByte(reader.GetOrdinal("Priority"));
                var details = reader.IsDBNull(reader.GetOrdinal("Details")) ? string.Empty : reader.GetString(reader.GetOrdinal("Details"));

                var record = new ServerFindingLog(checkId, priority, details);

                records.Add(record);
            }

            return records;
        }
    }
}