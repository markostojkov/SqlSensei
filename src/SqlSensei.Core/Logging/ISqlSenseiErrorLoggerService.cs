using System;
using System.Threading.Tasks;

namespace SqlSensei.Core
{
    public interface ISqlSenseiErrorLoggerService
    {
        Task Error(Exception exception, string message);
        Task Error(string message);
    }
}
