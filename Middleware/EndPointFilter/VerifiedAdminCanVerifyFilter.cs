using MACUTION.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Middleware.AddEndpointFilter
{
    
    public class VerifiedAdminCanVerifyFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var db = context.HttpContext.RequestServices.GetRequiredService<MacutionDatabase>();
            var id = context.HttpContext.Items["id"];
            if (id == null)
            {
                context.Result = new UnauthorizedObjectResult(new { status = "fail", message = "Unauthorized; could not find your identity." });
                return;
            }
            if (!int.TryParse(id.ToString(), out var userId))
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "Invalid user id in token." });
                return;
            }
            var currentUser = db.Users.Include(x => x.request).FirstOrDefault(x => x.Id == userId);
            if (currentUser == null)
            {
                context.Result = new NotFoundObjectResult(new { status = "fail", message = "User not found." });
                return;
            }
            if (currentUser.role != "ADMIN")
            {
                context.Result = new ObjectResult(new { status = "fail", message = "Only admin role can verify products." }) { StatusCode = 403 };
                return;
            }
            if (currentUser.request == null || !currentUser.request.verified_admin)
            {
                context.Result = new ObjectResult(new { status = "fail", message = "Only verified admins can verify products. Your admin account is not yet verified." }) { StatusCode = 403 };
                return;
            }
            context.HttpContext.Items["verified_admin_user_id"] = userId;
        }
    }
}
