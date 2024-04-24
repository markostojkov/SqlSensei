using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SqlSensei.Api.Insights;
using SqlSensei.Api.Services;
using SqlSensei.Core;

namespace SqlSensei.Api.Controllers
{
    [ApiController]
    [Route("[controller]/v{version:apiVersion}")]
    [ApiVersion((double)SqlSenseiConfigurationApiVersion.Version1)]
    public class SqlServerController(
        StoreLogsToDatabaseService storeLogsToDatabaseService,
        JobService jobService,
        SqlServerInsights serverInsights,
        ServersService serversService) : BaseController
    {
        public StoreLogsToDatabaseService StoreLogsToDatabaseService { get; } = storeLogsToDatabaseService;
        public JobService JobService { get; } = jobService;
        public SqlServerInsights ServerInsights { get; } = serverInsights;
        public ServersService ServersService { get; } = serversService;

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

        [HttpGet("servers")]
        public async Task<IActionResult> GetServers()
        {
            var result = await ServersService.GetServers();

            return OkOrError(result);
        }

        [HttpPost("servers")]
        public async Task<IActionResult> CreateServer([FromBody]CreateServerRequest req)
        {
            var result = await ServersService.CreateServer(req);

            return OkOrError(result);
        }

        [HttpGet("servers/{serverId:long}")]
        public async Task<IActionResult> GetServer(long serverId)
        {
            var result = await ServerInsights.GetServerInfo(serverId);

            return OkOrError(result);
        }

        [HttpGet("servers/{serverId:long}/insights")]
        public async Task<IActionResult> GetServerInsights(long serverId, [FromQuery]DateTime date)
        {
            var result = await ServerInsights.GetInsights(serverId, date);

            return OkOrError(result);
        }

        [HttpDelete("servers/{serverId:long}")]
        public async Task<IActionResult> DeleteServer(long serverId)
        {
            var result = await ServersService.DeleteServer(serverId);

            return OkOrError(result);
        }

        [HttpGet("servers/{serverId:long}/wait-stats")]
        public async Task<IActionResult> GetServerWaitStats(long serverId, [FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            var result = await ServerInsights.GetWaitStats(serverId, start, end);

            return OkOrError(result);
        }

        [HttpGet("servers/{serverId:long}/performance")]
        public async Task<IActionResult> GetServerPerformanceStats(long serverId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var result = await ServerInsights.GetPerformanceStats(serverId, start, end);

            return OkOrError(result);
        }
    }
}
