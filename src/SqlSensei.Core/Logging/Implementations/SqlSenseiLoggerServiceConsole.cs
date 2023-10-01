using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public class SqlSenseiLoggerServiceConsole : ISqlSenseiLoggerService
    {
        public Task Error(Exception exception, string message)
        {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public Task Error(string message)
        {
            Console.Error.WriteLine(message);
            return Task.CompletedTask;
        }

        public Task MaintenanceInformation(IEnumerable<IMaintenanceJobLog> jobLogs, string database)
        {
            var errorCount = jobLogs.Count(jobLog => jobLog.IsError);

            var csvContent = new StringBuilder();

            csvContent.AppendLine("Index,Statistic,IsError,ErrorMessage");

            foreach (var jobLog in jobLogs)
            {
                csvContent.AppendLine($"{jobLog.Index},{jobLog.Statistic},{jobLog.IsError},{jobLog.ErrorMessage}");
            }

            Console.WriteLine(csvContent.ToString());

            return Task.CompletedTask;
        }

        public Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> indexLogs, string database)
        {
            if (!indexLogs.Any())
            {
                return Task.CompletedTask;
            }

            var csvContent = new StringBuilder();

            csvContent.AppendLine("Database,TableName,MagicBenefitNumber,Impact,IndexDetails");

            foreach (var jobLog in indexLogs)
            {
                csvContent.AppendLine($"{jobLog.DatabaseName},{jobLog.TableName},{jobLog.MagicBenefitNumber},{jobLog.Impact},{jobLog.IndexDetails}");
            }

            Console.WriteLine(csvContent.ToString());

            return Task.CompletedTask;
        }
    }
}
