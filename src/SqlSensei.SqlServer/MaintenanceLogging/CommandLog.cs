using Microsoft.Data.SqlClient;

using SqlSensei.Core;

using System;
using System.Collections.Generic;

namespace SqlSensei.SqlServer.InformationGather
{
    public class CommandLog : IMaintenanceJobLog
    {
        public int ID { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string IndexName { get; set; }
        public byte? IndexType { get; set; }
        public string StatisticsName { get; set; }
        public int? PartitionNumber { get; set; }
        public string ExtendedInfo { get; set; }
        public string Command { get; set; }
        public string CommandType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string Index => IndexName ?? string.Empty;
        public string Statistic => StatisticsName ?? string.Empty;
        public bool IsError => ErrorNumber != 0;

        public static List<CommandLog> GetAll(SqlDataReader reader)
        {
            List<CommandLog> commandLogs = [];

            while (reader.Read())
            {
                CommandLog commandLog = new()
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    DatabaseName = reader.IsDBNull(reader.GetOrdinal("DatabaseName")) ? null : reader.GetString(reader.GetOrdinal("DatabaseName")),
                    SchemaName = reader.IsDBNull(reader.GetOrdinal("SchemaName")) ? null : reader.GetString(reader.GetOrdinal("SchemaName")),
                    ObjectName = reader.IsDBNull(reader.GetOrdinal("ObjectName")) ? null : reader.GetString(reader.GetOrdinal("ObjectName")),
                    ObjectType = reader.IsDBNull(reader.GetOrdinal("ObjectType")) ? null : reader.GetString(reader.GetOrdinal("ObjectType")),
                    IndexName = reader.IsDBNull(reader.GetOrdinal("IndexName")) ? null : reader.GetString(reader.GetOrdinal("IndexName")),
                    IndexType = reader.IsDBNull(reader.GetOrdinal("IndexType")) ? null : reader.GetByte(reader.GetOrdinal("IndexType")),
                    StatisticsName = reader.IsDBNull(reader.GetOrdinal("StatisticsName")) ? null : reader.GetString(reader.GetOrdinal("StatisticsName")),
                    PartitionNumber = reader.IsDBNull(reader.GetOrdinal("PartitionNumber")) ? null : reader.GetInt32(reader.GetOrdinal("PartitionNumber")),
                    ExtendedInfo = reader.IsDBNull(reader.GetOrdinal("ExtendedInfo")) ? null : reader.GetString(reader.GetOrdinal("ExtendedInfo")),
                    Command = reader.GetString(reader.GetOrdinal("Command")),
                    CommandType = reader.GetString(reader.GetOrdinal("CommandType")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                    EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                    ErrorNumber = reader.IsDBNull(reader.GetOrdinal("ErrorNumber")) ? null : reader.GetInt32(reader.GetOrdinal("ErrorNumber")),
                    ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? null : reader.GetString(reader.GetOrdinal("ErrorMessage"))
                };

                commandLogs.Add(commandLog);
            }

            return commandLogs;
        }
    }
}