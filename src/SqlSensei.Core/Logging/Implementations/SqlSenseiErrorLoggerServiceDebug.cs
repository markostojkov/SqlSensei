using System;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public class SqlSenseiErrorLoggerServiceDebug : ISqlSenseiErrorLoggerService
    {

        public SqlSenseiErrorLoggerServiceDebug()
        {
        }

        public Task Error(Exception exception, string message)
        {
            Console.WriteLine(message);

            Console.WriteLine(exception);

            return Task.CompletedTask;
        }

        public Task Error(string message)
        {
            Console.WriteLine(message);

            return Task.CompletedTask;
        }
    }

}
