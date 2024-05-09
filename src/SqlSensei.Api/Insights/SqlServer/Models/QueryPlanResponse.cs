namespace SqlSensei.Api.Insights
{
    public class QueryPlanResponse(string xmlPlan, string sqlText)
    {
        public string XmlPlan { get; } = xmlPlan;
        public string SqlText { get; } = sqlText;
    }
}
