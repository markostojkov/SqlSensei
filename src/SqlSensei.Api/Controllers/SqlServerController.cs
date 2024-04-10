using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SqlSensei.Api.Services;
using SqlSensei.Core;

namespace SqlSensei.Api.Controllers
{
    [ApiController]
    [Route("[controller]/v{version:apiVersion}")]
    [ApiVersion((double)SqlSenseiConfigurationApiVersion.Version1)]
    public class SqlServerController(StoreLogsToDatabaseService storeLogsToDatabaseService, JobService jobService) : BaseController
    {
        public StoreLogsToDatabaseService StoreLogsToDatabaseService { get; } = storeLogsToDatabaseService;
        public JobService JobService { get; } = jobService;

        [HttpPost("jobs/{jobId:long}/monitoring")]
        public async Task<IActionResult> PostMonitoringLogs(long jobId, [FromBody] MonitoringLogRequest request)
        {
            var result = await StoreLogsToDatabaseService.StoreMonitoringLogs(jobId, request);

            return OkOrError(result);
        }

        [HttpPost("jobs/{jobId:long}/maintenance")]
        public async Task<IActionResult> PostMaintenanceLogs(long jobId, [FromBody] MaintenanceLogRequest request)
        {
            var result = await StoreLogsToDatabaseService.StoreMaintenanceLogs(jobId, request);

            return OkOrError(result);
        }

        [HttpGet("can-execute-jobs")]
        public async Task<IActionResult> CanExecuteJobs()
        {
            var result = await JobService.CanExecuteJobs();

            return OkOrError(result);
        }
    }
}
