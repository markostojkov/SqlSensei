using SqlSensei.Core;

namespace SqlSensei.Api.Storage
{
    public class Server
    {
        public long Id { get; set; }
        public long CompanyFk { get; set; }
        public string Name { get; set; }
        public Guid ApiKey { get; set; }
        public SqlSenseiRunMaintenancePeriod DoMaintenancePeriod { get; set; }
        public SqlSenseiRunMonitoringPeriod DoMonitoringPeriod { get; set; }
        public virtual Company Company { get; set; }

        public Server(string name)
        {
            Name = name;
            ApiKey = Guid.NewGuid();
        }

        public Server()
        {
            Name = string.Empty;
        }
    }
}
