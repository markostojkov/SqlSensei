namespace SqlSensei.Api.CurrentCompany
{
    public class CurrentCompanyMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context, CurrentCompanyService currentCompanyService)
        {
            if (context.Request.Headers.TryGetValue("sqlsensei-token", out var token) && Guid.TryParse(token, out var apiKey))
            {
                currentCompanyService.SetCurrentCompany(apiKey);
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
