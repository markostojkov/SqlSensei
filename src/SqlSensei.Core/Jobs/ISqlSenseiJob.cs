using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface ISqlSenseiJob
    {
        void InstallMaintenanceAndMonitoringScripts();

        Task ExecuteMaintenanceJob(long jobId);

        Task ExecuteMonitoringJob(long jobId);

        void StartService();
    }
}