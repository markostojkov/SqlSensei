namespace SqlSensei.Api.Storage
{
    public class JobExecution
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; }
        public long ServerFk { get; set; }
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool MaintenanceErrorLast { get; set; }
        public bool MonitoringErrorLast { get; set; }
        public virtual Company Company { get; set; }
        public virtual Server Server { get; set; }
    }

    public enum JobType
    {
        Maintenance,
        Monitoring
    }

    public enum JobStatus
    {
        InProgress,
        Completed
    }
}
