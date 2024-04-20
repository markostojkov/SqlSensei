using System.Security.Claims;

namespace SqlSensei.Api.User
{
    public class CurrentUser
    {
        private const string ScopeUid = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public string? Identity { get; }

        public CurrentUser(ClaimsPrincipal? claimsPrincipal)
        {
            Identity = claimsPrincipal?.FindFirstValue(ScopeUid);
        }
    }
}
