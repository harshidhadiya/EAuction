using MACUTION.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Middleware.AddEndpointFilter
{
    public class acceptServiceVerifier : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var db=context.HttpContext.RequestServices.GetRequiredService<MacutionDatabase>();
           var id =context.HttpContext.Items["id"];
           if (id==null)
           {
            context.Result=new BadRequestObjectResult(new {status="fail",message="sorry maybe you are unauthorized we are not able to find you"});
            return ;
           }
           if (!int.TryParse(id.ToString(), out var userId))
            {
            context.Result=new BadRequestObjectResult(new {status="fail",message="Your Id in Token Is Not valid"});
            return ;
            }
           var current_user=db.Users.Where(x=>x.Id==userId).FirstOrDefault();
           Console.WriteLine(userId);
           if (current_user==null)
           {
            context.Result=new BadRequestObjectResult(new {status="fail",message="You are not exist here"});
            return ;
           }
           if(current_user.right_to_add == false)
            {
            context.Result=new BadRequestObjectResult(new {status="fail",message="sorry maybe you are unauthorized To adding user"});
            return ;
            }
           context.HttpContext.Items["Users"]=db.Users.Include(x => x.request).Include(x => x.given_access);
           context.HttpContext.Items["UserId"]=userId;
           context.HttpContext.Items["verfired"]=db.request_Admins;
           context.HttpContext.Items["admin_current_user"]=current_user;

        }
    }
}