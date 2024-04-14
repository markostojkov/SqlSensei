namespace SqlSensei.Api.Storage
{
    public class MonitoringJobServerWaitStatLog(long companyFk, long jobFk, string type, long timeInMs)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string Type { get; } = type;
        public long TimeInMs { get; } = timeInMs;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
