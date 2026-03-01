using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MACUTION.Middleware.AddEndpointFilter
{
    // Changed: IActionFilter -> IAsyncActionFilter for consistency with async pipeline
    public class IdVerifier : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var id = context.HttpContext.Items["id"];
            if (id == null)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "sorry maybe you are unauthorized we are not able to find you" });
                return;
            }
            if (!int.TryParse(id.ToString(), out var userId))
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "Your Id in Token Is Not valid" });
                return;
            }
            context.HttpContext.Items["UserId"] = userId;
            await next();
        }
    }
}
