namespace SqlSensei.Api.Insights
{
    public class InsightsResponse
    {
        public InsightsResponse(SqlServerCheck sqlServerCheck, SqlServerPerformanceCheck sqlServerPerformanceCheck, SqlServerIndexCheck indexCheck)
        {
            SqlServerCheck = sqlServerCheck;
            SqlServerPerformanceCheck = sqlServerPerformanceCheck;
            IndexCheck = indexCheck;
        }

        public SqlServerCheck SqlServerCheck { get; }
        public SqlServerPerformanceCheck SqlServerPerformanceCheck { get; }
        public SqlServerIndexCheck IndexCheck { get; }
    }
}
