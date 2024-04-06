namespace SqlSensei.Core
{
    public interface IMaintenanceJobLog : IDatabaseLog
    {
        public string Index { get; }
        public string Statistic { get; }
        public bool IsError { get; }
        public string ErrorMessage { get; }
    }
}