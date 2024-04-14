namespace SqlSensei.Api.Storage
{
    public class MonitoringJobServerFindingLog(long companyFk, long jobFk, int checkId, short priority, string details)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public short Priority { get; set; } = priority;
        public int CheckId { get; set; } = checkId;
        public string Details { get; set; } = details;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
