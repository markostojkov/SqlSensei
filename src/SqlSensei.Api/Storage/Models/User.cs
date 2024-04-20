namespace SqlSensei.Api.Storage
{
    public class User
    {
        public long CompanyFk { get; set; }
        public string Identifier { get; set; }
        public AuthProvider AuthProvider { get; set; }
        public virtual Company Company { get; set; }
    }

    public enum AuthProvider
    {
        Google
    }
}
