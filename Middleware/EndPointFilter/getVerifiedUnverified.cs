using System.Net.Http.Headers;
using MACUTION.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Middleware.AddEndpointFilter
{
    public class getVerifiedUnverified : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var id = context.HttpContext.Items["id"];
            var db=context.HttpContext.RequestServices.GetService<MacutionDatabase>();
            if (id == null)
            {
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "sorry maybe you are unauthorized we are not able to find you" });
                return;
            }

            if (!int.TryParse(id.ToString(), out var userId)) 
            { 
            context.Result = new BadRequestObjectResult(new { status = "fail", message = "Your Id in Token Is Not valid" }); 
            return ;
            }

            var data = db.Users.Include(x => x.given_access).Where(x => x.Id == userId).FirstOrDefault();
            if (data == null){
                context.Result = new BadRequestObjectResult(new { status = "fail", message = "You are Not Exist in the database" });
                return ;
            }
            if (data.given_access == null){
                context.Result=new NoContentResult();
                return ;
                }
                Console.WriteLine("entered here");
        }
    }
}