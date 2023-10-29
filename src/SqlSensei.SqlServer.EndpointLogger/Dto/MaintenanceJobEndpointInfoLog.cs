using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.EndpointLogger
{
    internal class MaintenanceJobLog : IMaintenanceJobLog
    {
        public string Index { get; set; }
        public string Statistic { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string DatabaseName { get; set; }

        public static MaintenanceJobLog MapFromDataReader(SqlDataReader reader)
        {
            var log = new MaintenanceJobLog
            {
                Index = reader.IsDBNull(reader.GetOrdinal("Index")) ? string.Empty : reader.GetString(reader.GetOrdinal("Index")),
                Statistic = reader.IsDBNull(reader.GetOrdinal("Statistic")) ? string.Empty : reader.GetString(reader.GetOrdinal("Statistic")),
                IsError = !reader.IsDBNull(reader.GetOrdinal("IsError")) && reader.GetBoolean(reader.GetOrdinal("IsError")),
                ErrorMessage = reader.IsDBNull(reader.GetOrdinal("ErrorMessage")) ? string.Empty : reader.GetString(reader.GetOrdinal("ErrorMessage")),
                DatabaseName = reader.IsDBNull(reader.GetOrdinal("DatabaseName")) ? string.Empty : reader.GetString(reader.GetOrdinal("DatabaseName"))
            };

            return log;
        }
    }
}
