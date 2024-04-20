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
    }

    public class ServerResponse(long id, string name)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
    }
}
