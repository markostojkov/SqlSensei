namespace SqlSensei.Api.Insights
{
    public class InsightsResponse(SqlServerCheck sqlServerCheck, SqlServerPerformanceCheck sqlServerPerformanceCheck, SqlServerIndexCheck indexCheck)
    {
        public SqlServerCheck SqlServerCheck { get; } = sqlServerCheck;
        public SqlServerPerformanceCheck SqlServerPerformanceCheck { get; } = sqlServerPerformanceCheck;
        public SqlServerIndexCheck IndexCheck { get; } = indexCheck;
    }
}
