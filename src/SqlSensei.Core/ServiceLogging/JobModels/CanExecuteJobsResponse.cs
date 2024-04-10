namespace SqlSensei.Core
{
    public class CanExecuteJobsResponse(
        bool canExecuteMaintenance,
        bool canExecuteMonitoringIndexUsage,
        bool canExecuteMonitoringIndexMissing,
        long maintenanceJobId,
        long monitoringIndexJobId)
    {
        public bool CanExecuteMaintenance { get; } = canExecuteMaintenance;
        public bool CanExecuteMonitoringIndexUsage { get; } = canExecuteMonitoringIndexUsage;
        public bool CanExecuteMonitoringIndexMissing { get; } = canExecuteMonitoringIndexMissing;
        public long MaintenanceJobId { get; } = maintenanceJobId;
        public long MonitoringIndexJobId { get; } = monitoringIndexJobId;
    }
}