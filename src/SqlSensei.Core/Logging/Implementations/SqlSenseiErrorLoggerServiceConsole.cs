using System;

namespace SqlSensei.Core
{
    public class SqlSenseiErrorLoggerServiceConsole : ISqlSenseiErrorLoggerService
    {
        public void Error(Exception exception, string message)
        {
            Console.WriteLine(message);

            Console.WriteLine(exception);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }
    }
}