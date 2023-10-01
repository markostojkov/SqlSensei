namespace SqlSensei.Core
{
    public interface IMonitoringJobIndexLog
    {
        public string DatabaseName { get; }
        public string TableName { get; }
        public long MagicBenefitNumber { get; }
        public string Impact { get; }
        public string IndexDetails { get; }
    }
}
