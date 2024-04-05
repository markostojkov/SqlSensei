namespace SqlSensei.Core
{
    public interface IMonitoringJobIndexMissingLog : IDatabaseLog
    {
        public string TableName { get; }
        public long MagicBenefitNumber { get; }
        public string Impact { get; }
        public string IndexDetails { get; }
    }
}
