using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class IndexJobEndpointInfoLog : IMonitoringJobIndexMissingLog
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public long MagicBenefitNumber { get; set; }
        public string Impact { get; set; }
        public string IndexDetails { get; set; }

        public static IndexJobEndpointInfoLog MapFromDataReader(SqlDataReader reader)
        {
            var log = new IndexJobEndpointInfoLog
            {
                DatabaseName = reader.IsDBNull(reader.GetOrdinal("DatabaseName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DatabaseName")),
                TableName = reader.IsDBNull(reader.GetOrdinal("TableName")) ? string.Empty : reader.GetString(reader.GetOrdinal("TableName")),
                MagicBenefitNumber = reader.IsDBNull(reader.GetOrdinal("MagicBenefitNumber")) ? 0 : reader.GetInt64(reader.GetOrdinal("MagicBenefitNumber")),
                Impact = reader.IsDBNull(reader.GetOrdinal("Impact")) ? string.Empty : reader.GetString(reader.GetOrdinal("Impact")),
                IndexDetails = reader.IsDBNull(reader.GetOrdinal("IndexDetails")) ? string.Empty : reader.GetString(reader.GetOrdinal("IndexDetails"))
            };

            return log;
        }
    }
}
