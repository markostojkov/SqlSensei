namespace SqlSensei.Core
{
    public interface IMonitoringJobServerPerformanceLogWaitStat
    {
        public string Type { get; }
        public long TimeInMs { get; }
    }
}