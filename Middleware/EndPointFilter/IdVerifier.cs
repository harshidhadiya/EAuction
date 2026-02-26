using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MACUTION.Middleware.AddEndpointFilter
{
    public class IdVerifier : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
           var id =context.HttpContext.Items["id"];
           if (id==null)
           {
            context.Result=new BadRequestObjectResult(new {status="fail",message="sorry maybe you are unauthorized we are not able to find you"});
            return ;
           }
           if (!int.TryParse(id.ToString(), out var userId))
            {
            context.Result=new BadRequestObjectResult(new {status="fail",message="Your Id in Token Is Not valid"});
            }
            Console.WriteLine("your user id"+userId);
           context.HttpContext.Items["UserId"]=userId;
        }
    }
}