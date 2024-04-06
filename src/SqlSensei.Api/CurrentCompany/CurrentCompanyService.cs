using SqlSensei.Api.Storage;
using SqlSensei.Api.Utils;

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

        public void SetCurrentCompany(Guid apiKey)
        {
            var company = DbContext.Companies
                .Where(c => c.ApiKey == apiKey)
                .Select(c => new CurrentCompany(c.Id, c.Name, c.ApiKey))
                .SingleOrDefault();

            CurrentCompany = company;
        }
    }

    public class CurrentCompany(long id, string name, Guid apiKey)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
        public Guid ApiKey { get; } = apiKey;
    }
}
