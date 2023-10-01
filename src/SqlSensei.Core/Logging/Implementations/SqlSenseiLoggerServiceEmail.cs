using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SqlSensei.Core.Logging.Email;

namespace SqlSensei.Core
{
    public class SqlSenseiLoggerServiceEmail : ISqlSenseiLoggerService
    {
        public IEmailService EmailService { get; }

        public SqlSenseiLoggerServiceEmail(IEmailService emailService)
        {
            EmailService = emailService;
        }

        public async Task MaintenanceInformation(IEnumerable<IMaintenanceJobLog> jobLogs, string database)
        {
            var errorCount = jobLogs.Count(jobLog => jobLog.IsError);

            string emailSubject = $"Maintenance Job Report For Database {database}";
            string emailBody = $"Total Error Rows Found: {errorCount}";

            var csvContent = new StringBuilder();

            csvContent.AppendLine("Index,Statistic,IsError,ErrorMessage");

            foreach (var jobLog in jobLogs)
            {
                csvContent.AppendLine($"{jobLog.Index},{jobLog.Statistic},{jobLog.IsError},{jobLog.ErrorMessage}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());

            await EmailService.SendEmailAsync(emailSubject, emailBody, "MaintenanceReport.csv", csvBytes, "text/csv");

        }

        public async Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> jobLogs, string database)
        {
            if (!jobLogs.Any())
            {
                return;
            }

            string emailSubject = $"Monitoring Job Report For Database {database}";
            string emailBody = $"High value missing index details";

            var csvContent = new StringBuilder();

            csvContent.AppendLine("Database,TableName,MagicBenefitNumber,Impact,IndexDetails");

            foreach (var jobLog in jobLogs)
            {
                csvContent.AppendLine($"{jobLog.DatabaseName},{jobLog.TableName},{jobLog.MagicBenefitNumber},{jobLog.Impact},{jobLog.IndexDetails}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvContent.ToString());

            await EmailService.SendEmailAsync(emailSubject, emailBody, "MonitoringReport.csv", csvBytes, "text/csv");

        }

        Task ISqlSenseiLoggerService.Error(Exception exception, string message)
        {
            return Task.CompletedTask;
        }

        Task ISqlSenseiLoggerService.Error(string message)
        {
            return Task.CompletedTask;
        }
    }
}
