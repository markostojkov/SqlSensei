#nullable enable

namespace SqlSensei.Core
{
    public interface IMonitoringJobServerLog
    {
        public string? DatabaseName { get; }
        public short Priority { get; }
        public int CheckId { get; }
        public string Details { get; }
    }
}