namespace SqlSensei.Core
{
    public interface IMonitoringJobServerPerformanceLogFinding
    {
        public int CheckId { get; }
        public short Priority { get; }
        public string Details { get; }
        public string Finding { get; }
    }
}