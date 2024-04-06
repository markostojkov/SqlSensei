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
    }
}