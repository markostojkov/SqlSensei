﻿using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using SqlSensei.Core;

namespace SqlSensei.SqlServer.InformationGather
{
    public class IndexLogUsage : IMonitoringJobIndexLogUsage
    {
        public IndexLogUsage(string databaseName, string indexName, string tableName, string indexDetails, string usage, long readsUsage, long writeUsage, bool isClusteredIndex)
        {
            DatabaseName = databaseName;
            IndexName = indexName;
            TableName = tableName;
            IndexDetails = indexDetails;
            Usage = usage;
            ReadsUsage = readsUsage;
            WriteUsage = writeUsage;
            IsClusteredIndex = isClusteredIndex;
            UserMessage = string.Empty;
        }

        public string DatabaseName { get; }

        public string IndexName { get; }

        public string TableName { get; }

        public string IndexDetails { get; }

        public string Usage { get; }

        public long ReadsUsage { get; }

        public long WriteUsage { get; }
        public bool IsClusteredIndex { get; }
        public string UserMessage { get; set; }

        public List<string> IndexColumns { get; set; }
        public List<string> IndexIncludeColumns { get; set; }

        public static List<IndexLogUsage> GetAll(SqlDataReader reader)
        {
            List<IndexLogUsage> records = new List<IndexLogUsage>();

            while (reader.Read())
            {
                var indexDefinitionKeys = reader.IsDBNull(reader.GetOrdinal("key_column_names_with_sort_order")) ? string.Empty : reader.GetString(reader.GetOrdinal("key_column_names_with_sort_order"));
                var indexDefinitionIncludes = reader.IsDBNull(reader.GetOrdinal("include_column_names")) ? string.Empty : reader.GetString(reader.GetOrdinal("include_column_names"));

                IndexLogUsage record = new IndexLogUsage(
                    reader.IsDBNull(reader.GetOrdinal("database_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("database_name")),
                    reader.IsDBNull(reader.GetOrdinal("index_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("index_name")),
                    reader.IsDBNull(reader.GetOrdinal("table_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("table_name")),
                    string.Format("{0} : {1}", indexDefinitionKeys, indexDefinitionIncludes),
                    reader.IsDBNull(reader.GetOrdinal("index_usage_summary")) ? string.Empty : reader.GetString(reader.GetOrdinal("index_usage_summary")),
                    reader.IsDBNull(reader.GetOrdinal("total_reads")) ? 0 : reader.GetInt64(reader.GetOrdinal("total_reads")),
                    reader.IsDBNull(reader.GetOrdinal("user_updates")) ? 0 : reader.GetInt64(reader.GetOrdinal("user_updates")),
                    reader.GetBoolean(reader.GetOrdinal("is_primary_key"))
                    );

                records.Add(record);
            }

            return records;
        }
    }
}
