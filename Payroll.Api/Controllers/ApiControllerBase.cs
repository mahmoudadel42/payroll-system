using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Common.Security;
using Payroll.Application.Resources;

namespace Payroll.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.AdminOrHR)]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected IActionResult FromResult(Result result) =>
            result.IsSuccess ? NoContent() : Problem(result.Error!);

        protected IActionResult FromResult<T>(Result<T> result) =>
            result.IsSuccess ? Ok(result.Value) : Problem(result.Error!);

        protected IActionResult FromCreated<T>(Result<T> result, string actionName, Func<T, object> routeValues) =>
            result.IsSuccess
                ? CreatedAtAction(actionName, routeValues(result.Value!), result.Value)
                : Problem(result.Error!);

        private ObjectResult Problem(Error error)
        {
            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.BusinessRule => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = AppMessages.Get(error.Code, error.Message)
            };
            problem.Extensions["code"] = error.Code;
            return new ObjectResult(problem) { StatusCode = status };
        }
    }
}
