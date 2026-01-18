namespace TaskTrackerAPI.Infrastructure
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    namespace TaskTrackerAPI.Middleware
    {
        public class ValidationFilter : IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (!context.ModelState.IsValid)
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation Error",
                        Detail = "One or more validation errors occurred",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Instance = context.HttpContext.Request.Path,
                        Extensions = { ["errors"] = errors }
                    };

                    context.Result = new BadRequestObjectResult(problemDetails);
                }
            }

            public void OnActionExecuted(ActionExecutedContext context) { }
        }
    }
}
