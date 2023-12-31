﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public class SqlSenseiLoggerServiceConsole : ISqlSenseiLoggerService
    {
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

        public Task MonitoringInformation(IEnumerable<IMonitoringJobIndexLog> indexLogs, IEnumerable<IMonitoringJobIndexLogUsage> indexLogsUsage, string database)
        {
            if (!indexLogs.Any() && !indexLogsUsage.Any())
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

            var csvContent2 = new StringBuilder();

            csvContent2.AppendLine("Database,TableName,IndexName,IndexDetails,Usage,UserMessage");

            foreach (var jobLog in indexLogsUsage)
            {
                csvContent.AppendLine($"{jobLog.DatabaseName},{jobLog.TableName},{jobLog.IndexName},{jobLog.Usage},{jobLog.UserMessage}");
            }

            Console.WriteLine(csvContent2.ToString());

            return Task.CompletedTask;
        }
    }
}
