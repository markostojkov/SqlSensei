using System;

namespace SqlSensei.Core
{
    public static class EnvHelpers
    {
        public static bool IsRelease()
        {
#if RELEASE
            return true;
#else
            return false;
#endif
        }

        public static Uri ApiUri(SqlSenseiConfigurationApiVersion apiVersion)
        {
            if (IsRelease())
            {
                return new Uri($"https://api.sqlsensei.net/sqlserver/v{Convert.ToInt32(apiVersion)}/");
            }
            else
            {
                return new Uri($"http://localhost:4000/sqlserver/v{Convert.ToInt32(apiVersion)}/");
            }
        }

        public static int DelayForJob()
        {
            if (IsRelease())
            {
                return 60000;
            }
            else
            {
                return 10000;
            }
        }
    }
}