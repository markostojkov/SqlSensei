using Microsoft.EntityFrameworkCore;
using SqlSensei.Api.Storage;
using SqlSensei.Api.User;
using System.Net;

namespace SqlSensei.Api.CurrentCompany
{
    public class CurrentCompanyMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context, SqlSenseiDbContext dbContext, CurrentCompanyService currentCompanyService, CurrentUser currentUser)
        {
            var hasToken = false;
            var hasUser = false;

            if (context.Request.Headers.TryGetValue("sqlsensei-token", out var token) && Guid.TryParse(token, out var apiKey))
            {
                await currentCompanyService.SetCurrentCompany(apiKey);

                hasToken = true;
            }

            var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Identifier == currentUser.Identity);

            if (dbUser is null && currentUser.Identity is not null)
            {
                var dbCompany = new Company(currentUser.Identity ?? Guid.NewGuid().ToString());

                dbContext.Companies.Add(dbCompany);

                dbUser = new Storage.User()
                {
                    Company = dbCompany,
                    AuthProvider = AuthProvider.Google,
                    Identifier = currentUser.Identity
                };

                dbContext.Users.Add(dbUser);

                await dbContext.SaveChangesAsync();
            }

            if (dbUser is not null)
            {
                await currentCompanyService.SetCurrentCompany(dbUser.CompanyFk);

                hasUser = true;
            }

            if (hasUser && hasToken)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                return;
            }

            await _next(context);
        }
    }

    public static class CurrentCompanyMiddlewareExtensions
    {
        public static IApplicationBuilder UseCurrentCompanyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CurrentCompanyMiddleware>();
        }
    }
}
