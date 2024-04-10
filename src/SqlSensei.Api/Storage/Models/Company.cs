using SqlSensei.Core;

namespace SqlSensei.Api.Storage
{
    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Guid ApiKey { get; set; }
        public SqlSenseiRunMaintenancePeriod DoMaintenancePeriod { get; set; }
        public SqlSenseiRunMonitoringPeriod DoMonitoringPeriod { get; set; }

        public Company(string name)
        {
            Name = name;
            ApiKey = Guid.NewGuid();
        }
    }
}
