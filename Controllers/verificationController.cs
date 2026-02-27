using MACUTION.Data;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class verificationController : ControllerBase
    {
        private MacutionDatabase db;
        public verificationController(MacutionDatabase db)
        {
            this.db = db;
        }
        [HttpGet("getallproduct")]
        [Authorize(Policy = "admin")]
        public ActionResult getAllProduct()
        {
            var id = HttpContext.Items["id"];
            if (id == null)
            {
                return BadRequest(new { status = "fail", message = "sorry maybe you are unauthorized we are not able to find you" });
            }
            if (!int.TryParse(id.ToString(), out var userId))
            {
                return BadRequest(new { status = "fail", message = "We Are not able to parse your id which is in the token" });
            }
            var current_user = db.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (current_user == null)
            {
                return BadRequest(new { status = "fail", message = "You are not exist here" });
            }
            var last = db.Products.Include(x => x.user).Where(x => (x.verifier == null || (x.verifier != null && x.verifier.isverified == false))).OrderByDescending(x=>x.creation_date).Select(x =>
            new allUnverifiedProduct_responce { buydate = x.Buy_Date, Id = x.Id, productname = x.product_name, ownerDetails = x.user != null ? new supportClass(x.user.Id, x.user.Name, x.user.Email, x.user.Address, x.user.ProfileImageUrl) :
             null }).DefaultIfEmpty();
         



            return Ok(last);
        }
    }
}