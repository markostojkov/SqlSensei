using System;
using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.InformationGather
{
    public class IndexLog : IMonitoringJobIndexMissingLog
    {
        public int ID { get; set; }
        public Guid RunID { get; set; }
        public DateTime RunDateTime { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public long MagicBenefitNumber { get; set; }
        public string MissingIndexDetails { get; set; }
        public decimal AvgTotalUserCost { get; set; }
        public decimal AvgUserImpact { get; set; }
        public long UserSeeks { get; set; }
        public long UserScans { get; set; }
        public long UniqueCompiles { get; set; }
        public string IndexEstimatedImpact { get; set; }
        public string CreateTSQL { get; set; }
        public string MoreInfo { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsLow { get; set; }
        public string Impact => IndexEstimatedImpact;
        public string IndexDetails => MissingIndexDetails;

        public static List<IndexLog> GetAll(SqlDataReader reader)
        {
            List<IndexLog> records = [];

            while (reader.Read())
            {
                IndexLog record = new()
                {
                    ID = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    RunID = reader.IsDBNull(reader.GetOrdinal("run_id")) ? Guid.Empty : reader.GetGuid(reader.GetOrdinal("run_id")),
                    RunDateTime = reader.IsDBNull(reader.GetOrdinal("run_datetime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("run_datetime")),
                    ServerName = reader.IsDBNull(reader.GetOrdinal("server_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("server_name")),
                    DatabaseName = reader.IsDBNull(reader.GetOrdinal("database_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("database_name")),
                    SchemaName = reader.IsDBNull(reader.GetOrdinal("schema_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("schema_name")),
                    TableName = reader.IsDBNull(reader.GetOrdinal("table_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("table_name")),
                    MagicBenefitNumber = reader.IsDBNull(reader.GetOrdinal("magic_benefit_number")) ? 0 : reader.GetInt64(reader.GetOrdinal("magic_benefit_number")),
                    MissingIndexDetails = reader.IsDBNull(reader.GetOrdinal("missing_index_details")) ? string.Empty : reader.GetString(reader.GetOrdinal("missing_index_details")),
                    AvgTotalUserCost = reader.IsDBNull(reader.GetOrdinal("avg_total_user_cost")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avg_total_user_cost")),
                    AvgUserImpact = reader.IsDBNull(reader.GetOrdinal("avg_user_impact")) ? 0 : reader.GetDecimal(reader.GetOrdinal("avg_user_impact")),
                    UserSeeks = reader.IsDBNull(reader.GetOrdinal("user_seeks")) ? 0 : reader.GetInt64(reader.GetOrdinal("user_seeks")),
                    UserScans = reader.IsDBNull(reader.GetOrdinal("user_scans")) ? 0 : reader.GetInt64(reader.GetOrdinal("user_scans")),
                    UniqueCompiles = reader.IsDBNull(reader.GetOrdinal("unique_compiles")) ? 0 : reader.GetInt64(reader.GetOrdinal("unique_compiles")),
                    IndexEstimatedImpact = reader.IsDBNull(reader.GetOrdinal("index_estimated_impact")) ? string.Empty : reader.GetString(reader.GetOrdinal("index_estimated_impact")),
                    CreateTSQL = reader.IsDBNull(reader.GetOrdinal("create_tsql")) ? string.Empty : reader.GetString(reader.GetOrdinal("create_tsql")),
                    MoreInfo = reader.IsDBNull(reader.GetOrdinal("more_info")) ? string.Empty : reader.GetString(reader.GetOrdinal("more_info")),
                    DisplayOrder = reader.IsDBNull(reader.GetOrdinal("display_order")) ? 0 : reader.GetInt32(reader.GetOrdinal("display_order")),
                    IsLow = reader.IsDBNull(reader.GetOrdinal("is_low")) ? false : reader.GetBoolean(reader.GetOrdinal("is_low"))
                };

                records.Add(record);
            }

            return records;
        }
    }
}
