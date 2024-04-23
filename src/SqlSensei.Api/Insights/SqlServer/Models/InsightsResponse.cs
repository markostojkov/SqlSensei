namespace SqlSensei.Api.Insights
{
    public class InsightsResponse(long id, string name, Guid apiKey, SqlServerCheck sqlServerCheck, SqlServerPerformanceCheck sqlServerPerformanceCheck, SqlServerIndexCheck indexCheck)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public Guid ApiKey { get; } = apiKey;
        public SqlServerCheck SqlServerCheck { get; } = sqlServerCheck;
        public SqlServerPerformanceCheck SqlServerPerformanceCheck { get; } = sqlServerPerformanceCheck;
        public SqlServerIndexCheck IndexCheck { get; } = indexCheck;
    }
}
