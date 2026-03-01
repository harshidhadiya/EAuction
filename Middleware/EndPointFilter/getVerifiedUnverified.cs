using MACUTION.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Middleware.AddEndpointFilter
{
    // Changed: IActionFilter -> IAsyncActionFilter, sync DB -> async for multithread/async
    public class getVerifiedUnverified : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var id = context.HttpContext.Items["id"];
            var db = context.HttpContext.RequestServices.GetService<MacutionDatabase>();
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
            if (db == null)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "Service not available" });
                return;
            }
            var data = await db.Users.AsNoTracking().Include(x => x.given_access).FirstOrDefaultAsync(x => x.Id == userId);
            if (data == null)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "You are Not Exist in the database" });
                return;
            }
            if (data.given_access == null)
            {
                context.Result = new NoContentResult();
                return;
            }
            await next();
        }
    }
}
