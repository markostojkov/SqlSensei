using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class IndexUsageJobEndpointInfoLog : IMonitoringJobIndexUsageLog
    {
        public bool IsClusteredIndex { get; set; }

        public string DatabaseName { get; set; }

        public string IndexName { get; set; }

        public string TableName { get; set; }

        public string IndexDetails { get; set; }

        public string Usage { get; set; }

        public long ReadsUsage { get; set; }

        public long WriteUsage { get; set; }

        public string UserMessage { get; set; }
        public List<string> IndexColumns { get; set; }
        public List<string> IndexIncludeColumns { get; set; }

        public static IndexUsageJobEndpointInfoLog MapFromDataReader(SqlDataReader reader)
        {
            var log = new IndexUsageJobEndpointInfoLog
            {
                DatabaseName = reader.IsDBNull(reader.GetOrdinal("DatabaseName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DatabaseName")),
                TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? string.Empty : reader.GetString(reader.GetOrdinal("TableName")),
                IndexName = reader.IsDBNull(reader.GetOrdinal("IndexName")) ? string.Empty : reader.GetString(reader.GetOrdinal("IndexName")),
                IndexDetails = reader.IsDBNull(reader.GetOrdinal("IndexDetails")) ? string.Empty : reader.GetString(reader.GetOrdinal("IndexDetails")),
                Usage = reader.IsDBNull(reader.GetOrdinal("Usage")) ? string.Empty : reader.GetString(reader.GetOrdinal("Usage")),
                UserMessage = reader.IsDBNull(reader.GetOrdinal("UserMessage")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserMessage")),
                IsClusteredIndex = reader.GetBoolean(reader.GetOrdinal("IsClusteredIndex"))
            };

            return log;
        }
    }
}
