using System.Collections.Generic;

namespace SqlSensei.Core
{
    public interface IMonitoringJobIndexUsageLog : IDatabaseLog
    {
        public bool IsClusteredIndex { get; }
        public string IndexName { get; }
        public string TableName { get; }
        public string IndexDetails { get; }
        public string Usage { get; }
        public long ReadsUsage { get; }
        public long WriteUsage { get; }
        public string UserMessage { get; set; }
        public List<string> IndexColumns { get; set; }
        public List<string> IndexIncludeColumns { get; set; }
    }
}
