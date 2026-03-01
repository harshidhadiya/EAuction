using MACUTION.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Middleware.AddEndpointFilter
{
    // Changed: IActionFilter -> IAsyncActionFilter, sync DB -> async (ToListAsync) for multithread/async
    public class acceptServiceVerifier : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<MacutionDatabase>();
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
            var current_user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (current_user == null)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "You are not exist here" });
                return;
            }
            if (current_user.right_to_add == false)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "sorry maybe you are unauthorized To adding user" });
                return;
            }
            // No AsNoTracking: controller may modify entities and call SaveChangesAsync
            var users = await db.Users.Include(x => x.request).Include(x => x.given_access).ToListAsync();
            context.HttpContext.Items["Users"] = users;
            context.HttpContext.Items["UserId"] = userId;
            context.HttpContext.Items["verfired"] = db.request_Admins;
            context.HttpContext.Items["admin_current_user"] = current_user;
            await next();
        }
    }
}
