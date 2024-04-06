using Microsoft.AspNetCore.Mvc;
using SqlSensei.Api.Utils;

namespace SqlSensei.Api.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult OkOrError<T>(Result<T> result)
        {
            var errorResponse = GetErrorResponse(result);

            return errorResponse ?? base.Ok(result);
        }

        protected IActionResult OkOrError(ResultCommonLogic result)
        {
            var errorResponse = GetErrorResponse(result);

            return errorResponse ?? base.Ok(result);
        }

        protected IActionResult OkEmptyOrError<T>(Result<T> result)
        {
            var errorResponse = GetErrorResponse(result);

            return errorResponse ?? base.Ok(Result.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Result.Ok(result));
        }

        private static IActionResult? GetErrorResponse(ResultCommonLogic result)
        {
            if (result.IsFailure)
            {
                IActionResult errorResponse = new ObjectResult(result)
                {
                    DeclaredType = typeof(ResultCommonLogic),
                    StatusCode = (int)result.HttpStatusCode
                };

                return errorResponse;
            }

            return null;
        }
    }
}