namespace SqlSensei.Api.Storage
{
    public class MonitoringJobIndexUsageLog(long companyFk, string databaseName, bool isClusteredIndex, string indexName, string tableName, string indexDetails, string usage, long readsUsage, long writeUsage)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public Company Company { get; set; } = new Company();
        public string DatabaseName { get; set; } = databaseName;
        public bool IsClusteredIndex { get; set; } = isClusteredIndex;
        public string IndexName { get; set; } = indexName;
        public string TableName { get; set; } = tableName;
        public string IndexDetails { get; set; } = indexDetails;
        public string Usage { get; set; } = usage;
        public long ReadsUsage { get; set; } = readsUsage;
        public long WriteUsage { get; set; } = writeUsage;
    }
}
