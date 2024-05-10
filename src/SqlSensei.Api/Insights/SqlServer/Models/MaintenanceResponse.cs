namespace SqlSensei.Api.Insights
{
    public class MaintenanceResponse(string databaseName, string? index, string? statistic, bool isError, string? errorMessage)
    {
        public string DatabaseName { get; } = databaseName;
        public string? Index { get; } = index;
        public string? Statistic { get; } = statistic;
        public bool IsError { get; } = isError;
        public string? ErrorMessage { get; } = errorMessage;
    }
}
