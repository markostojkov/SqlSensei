namespace SqlSensei.Api.Storage
{
    public class MonitoringJobIndexUsageLog(long companyFk, long jobFk, string databaseName, bool isClusteredIndex, string indexName, string tableName, string indexDetails, string usage, long readsUsage, long writeUsage)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string DatabaseName { get; set; } = databaseName;
        public bool IsClusteredIndex { get; set; } = isClusteredIndex;
        public string IndexName { get; set; } = indexName;
        public string TableName { get; set; } = tableName;
        public string IndexDetails { get; set; } = indexDetails;
        public string Usage { get; set; } = usage;
        public long ReadsUsage { get; set; } = readsUsage;
        public long WriteUsage { get; set; } = writeUsage;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
        public List<string> IndexColumns { get; set; }
        public List<string> IndexIncludeColumns { get; set; }
    }
}
