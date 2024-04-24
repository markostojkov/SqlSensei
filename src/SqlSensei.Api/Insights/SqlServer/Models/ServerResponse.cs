namespace SqlSensei.Api.Insights
{
    public class ServerResponse(long id, string name, Guid apiKey, SqlServerInsightsServerInfo serverInfo)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public Guid ApiKey { get; } = apiKey;
        public SqlServerInsightsServerInfo ServerInfo { get; } = serverInfo;
    }
}
