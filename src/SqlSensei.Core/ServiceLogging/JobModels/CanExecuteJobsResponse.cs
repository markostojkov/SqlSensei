namespace SqlSensei.Core
{
    public class CanExecuteJobsResponse(
        bool canExecuteMaintenance,
        bool canExecuteMonitoring,
        long maintenanceJobId,
        long monitoringJobId)
    {
        public bool CanExecuteMaintenance { get; } = canExecuteMaintenance;
        public bool CanExecuteMonitoring { get; } = canExecuteMonitoring;
        public long MaintenanceJobId { get; } = maintenanceJobId;
        public long MonitoringJobId { get; } = monitoringJobId;
    }
}