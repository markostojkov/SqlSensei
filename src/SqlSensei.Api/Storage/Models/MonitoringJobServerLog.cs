namespace SqlSensei.Api.Storage
{
    public class MonitoringJobServerLog(long companyFk, long jobFk, string? databaseName, short priority, int checkId, string details)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string? DatabaseName { get; set; } = databaseName;
        public short Priority { get; set; } = priority;
        public int CheckId { get; set; } = checkId;
        public string Details { get; set; } = details;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
