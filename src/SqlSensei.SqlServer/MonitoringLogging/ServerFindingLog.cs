using SqlSensei.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace SqlSensei.SqlServer.InformationGather
{
    public class ServerFindingLog(int checkId, short priority, string details, string finding) : IMonitoringJobServerPerformanceLogFinding
    {
        public int CheckId { get; } = checkId;
        public short Priority { get; } = priority;
        public string Details { get; } = details;
        public string Finding { get; } = finding;

        public static List<ServerFindingLog> GetAll(DataTable dataTable)
        {
            var records = new List<ServerFindingLog>();

            foreach (DataRow row in dataTable.Rows)
            {
                var checkId = row.IsNull("CheckID") ? -1 : Convert.ToInt32(row["CheckID"]);
                var priority = row.IsNull("Priority") ? (short)-1 : Convert.ToByte(row["Priority"]);
                var details = row.IsNull("Details") ? string.Empty : Convert.ToString(row["Details"]);
                var finding = row.IsNull("Finding") ? string.Empty : Convert.ToString(row["Finding"]);

                var record = new ServerFindingLog(checkId, priority, details, finding);

                records.Add(record);
            }

            return records;
        }
    }
}