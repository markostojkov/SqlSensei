using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface ISqlSenseiJob
    {
        void InstallMaintenanceAndMonitoringScripts();

        Task ExecuteMaintenanceJob();

        Task ExecuteMonitoringJob();
    }
}