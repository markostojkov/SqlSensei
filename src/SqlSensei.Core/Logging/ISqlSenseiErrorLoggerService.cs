using System;

namespace SqlSensei.Core
{
    public interface ISqlSenseiErrorLoggerService
    {
        void Error(Exception exception, string message);
        void Error(string message);
    }
}