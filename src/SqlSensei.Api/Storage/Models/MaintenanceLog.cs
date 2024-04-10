namespace SqlSensei.Api.Storage
{
    public class MaintenanceLog(
        long companyFk,
        long jobFk,
        string databaseName,
        string? index = null,
        string? statistic = null,
        bool isError = false,
        string? errorMessage = null)
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; } = companyFk;
        public long JobFk { get; set; } = jobFk;
        public string DatabaseName { get; set; } = databaseName;
        public string? Index { get; set; } = index;
        public string? Statistic { get; set; } = statistic;
        public bool IsError { get; set; } = isError;
        public string? ErrorMessage { get; set; } = errorMessage;
        public virtual Company Company { get; set; }
        public virtual JobExecution Job { get; set; }
    }
}
