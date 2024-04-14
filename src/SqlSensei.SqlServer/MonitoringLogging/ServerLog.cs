#nullable enable

using Microsoft.Data.SqlClient;

using SqlSensei.Core;
using System.Collections.Generic;

namespace SqlSensei.SqlServer.InformationGather
{
    public class ServerLog(string? databaseName, short priority, int checkId, string details) : IMonitoringJobServerLog
    {
        public string? DatabaseName { get; } = databaseName;
        public short Priority { get; } = priority;
        public int CheckId { get; } = checkId;
        public string Details { get; } = details;

        public static List<ServerLog> GetAll(SqlDataReader reader)
        {
            List<ServerLog> records = [];

            while (reader.Read())
            {
                var databaseName = reader.IsDBNull(reader.GetOrdinal("DatabaseName")) ? null : reader.GetString(reader.GetOrdinal("DatabaseName"));
                var priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? (short)-1 : reader.GetByte(reader.GetOrdinal("Priority"));
                var checkId = reader.IsDBNull(reader.GetOrdinal("CheckID")) ? -1 : reader.GetInt32(reader.GetOrdinal("CheckID"));
                var details = reader.IsDBNull(reader.GetOrdinal("Details")) ? string.Empty : reader.GetString(reader.GetOrdinal("Details"));

                var record = new ServerLog(databaseName, priority, checkId, details);

                records.Add(record);
            }

            return records;
        }
    }
}