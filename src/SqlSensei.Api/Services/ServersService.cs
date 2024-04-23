using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.CurrentCompany;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.Services
{
    public class ServersService(SqlSenseiDbContext dbContext, CurrentCompanyService company)
    {
        public SqlSenseiDbContext DbContext { get; } = dbContext;
        public CurrentCompanyService Company { get; } = company;

        public async Task<Result<IEnumerable<ServerResponse>>> GetServers()
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<IEnumerable<ServerResponse>>(companyResult);
            }

            var servers = await DbContext.Servers
                .Where(x => x.CompanyFk == companyResult.Value.Id)
                .ToListAsync();

            return Result.Ok(servers.Select(x => new ServerResponse(x.Id, x.Name)));
        }

        public async Task<Result> DeleteServer(long serverId)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<int>(companyResult);
            }

            var server = await DbContext.Servers
                .Where(x => x.CompanyFk == companyResult.Value.Id && x.Id == serverId)
                .FirstOrDefaultAsync();

            if (server == null)
            {
                return Result.NotFound<int>(ResultCodes.ServerNotFound);
            }

            DbContext.Servers.Remove(server);

            await DbContext.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result<long>> CreateServer(CreateServerRequest request)
        {
            var companyResult = Company.GetCurrentCompany();

            if (companyResult.IsFailure)
            {
                return Result.FromError<long>(companyResult);
            }

            var serverWithNameExists = await DbContext.Servers.FirstOrDefaultAsync(x => x.Name == request.Name && x.CompanyFk == companyResult.Value.Id);

            if (serverWithNameExists != null)
            {
                return Result.Conflicted<long>(ResultCodes.ServerWithNameExists);
            }

            var newServer = new Server
            {
                Name = request.Name,
                CompanyFk = companyResult.Value.Id,
                DoMonitoringPeriod = request.MonitoringPeriod,
                DoMaintenancePeriod = request.MaintenancePeriod,
                ApiKey = Guid.NewGuid()
            };

            DbContext.Servers.Add(newServer);
            await DbContext.SaveChangesAsync();

            return Result.Ok(newServer.Id);
        }
    }

    public class ServerResponse(long id, string name)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
    }

    public class CreateServerRequest(string name, SqlSenseiRunMonitoringPeriod monitoringPeriod, SqlSenseiRunMaintenancePeriod maintenancePeriod)
    {
        public string Name { get; } = name;
        public SqlSenseiRunMonitoringPeriod MonitoringPeriod { get; } = monitoringPeriod;
        public SqlSenseiRunMaintenancePeriod MaintenancePeriod { get; } = maintenancePeriod;
    }
}
