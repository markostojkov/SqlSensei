using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SqlSensei.Api.Services;
using SqlSensei.Core;

namespace SqlSensei.Api.Controllers
{
    [ApiController]
    [Route("[controller]/{version:apiVersion}")]
    [ApiVersion(SqlSenseiConfigurationApiVersion.Version1)]
    public class SqlServerController(StoreLogsToDatabaseService storeLogsToDatabaseService) : BaseController
    {
        public StoreLogsToDatabaseService StoreLogsToDatabaseService { get; } = storeLogsToDatabaseService;

        [HttpPost("monitoring/log")]
        public async Task<IActionResult> PostMonitoringLogs([FromBody] MonitoringLogRequest request)
        {
            var result = await StoreLogsToDatabaseService.StoreMonitoringLogs(request);

            return OkOrError(result);
        }

        [HttpPost("maintenance/log")]
        public async Task<IActionResult> PostMaintenanceLogs([FromBody] MaintenanceLogRequest request)
        {
            var result = await StoreLogsToDatabaseService.StoreMaintenanceLogs(request);

            return OkOrError(result);
        }
    }
}
