namespace SqlSensei.Api.Storage
{
    public class MonitoringJobIndexMissingLog(long companyFk, long jobFk, string databaseName, string tableName, long magicBenefitNumber, string impact, string indexDetails)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string DatabaseName { get; set; } = databaseName;
        public string TableName { get; set; } = tableName;
        public long MagicBenefitNumber { get; set; } = magicBenefitNumber;
        public string Impact { get; set; } = impact;
        public string IndexDetails { get; set; } = indexDetails;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
