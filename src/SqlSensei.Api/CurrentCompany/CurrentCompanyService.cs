using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.Storage;
using SqlSensei.Core;

namespace SqlSensei.Api.CurrentCompany
{
    public class CurrentCompanyService(SqlSenseiDbContext dbContext)
    {
        private readonly SqlSenseiDbContext DbContext = dbContext;
        private CurrentCompany? CurrentCompany;

        public Result<CurrentCompany> GetCurrentCompany()
        {
            if (CurrentCompany is null)
            {
                return Result.NotFound<CurrentCompany>(ResultCodes.CurrentCompanyNotFound);
            }

            return Result.Ok(CurrentCompany);
        }

        public async Task SetCurrentCompany(Guid apiKey)
        {
            var company = await DbContext.Companies
                .Where(c => c.ApiKey == apiKey)
                .Select(c => new CurrentCompany(c.Id, c.Name, c.ApiKey, c.DoMaintenancePeriod, c.DoMonitoringPeriod))
                .SingleOrDefaultAsync();

            CurrentCompany = company;
        }
    }

    public class CurrentCompany(
        long id,
        string name,
        Guid apiKey,
        SqlSenseiRunMaintenancePeriod maintenancePeriod,
        SqlSenseiRunMonitoringPeriod monitoringPeriod)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public Guid ApiKey { get; } = apiKey;
        public SqlSenseiRunMaintenancePeriod DoMaintenancePeriod { get; } = maintenancePeriod;
        public SqlSenseiRunMonitoringPeriod DoMonitoringPeriod { get; } = monitoringPeriod;
    }
}
