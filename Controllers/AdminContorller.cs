using MACUTION.Data;
using MACUTION.Middleware.AddEndpointFilter;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Name;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class adminController : ControllerBase
    {
        private MacutionDatabase db;
        private ItokenGeneration token;
        PasswordHasher<object> hash;
        public adminController(MacutionDatabase db, ItokenGeneration itoken)
        {
            this.db = db;
            this.hash = new PasswordHasher<object>();
            this.token = itoken;
        }
        [HttpPost("request/signup")]
        public ActionResult requestAdminSignup(signupRequestDto obj)
        {
            dynamic response;
            try
            {
                var data = db.Users.Where(x => x.Email == obj.Email).FirstOrDefault();
                if (data != null && data.role == "USER")
                {
                    return BadRequest(new { status = "fail", message = "You have already register as the user" });
                }
                else if (data != null)
                {

                    return BadRequest(new { status = "fail", message = "You have already register try to login" });
                }
                string hashedPassword = hash.HashPassword(new object(), obj.Password);
                var user = db.Users.Add(new User { Address = obj.Address, createdAt = DateTime.Now, Email = obj.Email, MobileNumber = obj.MobileNumber, Name = obj.Name, Password = hashedPassword, role = obj.role, ProfileImageUrl = obj.ProfileImageUrl ?? "" });
                db.SaveChanges();
                var currentuser = user.Entity;
                if (currentuser == null)
                {
                    return BadRequest(new { status = "fail", message = "Try to aganin sign up server erro " });
                }
                response = new { status = "success", data = new { Address = obj.Address, createdAt = DateTime.Now, Email = obj.Email, MobileNumber = obj.MobileNumber, Name = obj.Name, role = obj.role, ProfileImageUrl = obj.ProfileImageUrl ?? "", token = token.getToken(obj.Name, obj.role, currentuser.Id.ToString()) } };
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(response);
        }
        [HttpPost("login", Name = "login")]
        public ActionResult login(loginRequestDto obj)
        {
            loginResponceDto responce = new loginResponceDto();
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "success", message = "add all required fields" });
            }
            List<String> error = new List<string>();

            if (obj.email == "")
            {
                error.Add("Your Mail Shouldn't be empty");
            }
            if (obj.password == "")
            {
                error.Add("Your password requrire her authenticate to you");
            }
            if (obj.role == "") error.Add("Your Role shouldn't be empty");
            if (obj.role == "USER") error.Add("You have to require login as the admin");
            if (error.Count != 0)
            {
                return BadRequest(new { status = "fail", message = string.Join(", ", error) });
            }
            var data=db.Users.Where(x=>x.Email==obj.email).FirstOrDefault();
            if (data == null || data.role =="USER")
            {
                return BadRequest(new { status = "fail", message = "Sorry But Your mail Doesn't exist Or You Already Stored As user role"});
            }
            if (hash.VerifyHashedPassword(new object(),data.Password,obj.password)==PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { status = "fail", message = "Your Password is wrong"});
            }

            responce.id=data.Id;
            responce.imageurl=data.ProfileImageUrl;
            responce.token=token.getToken(data.Name,"ADMIN",data.Id.ToString());
            responce.request_accepted=data.right_to_add;
            responce.name=data.Name;
            responce.email=data.Email;
            return Ok(responce);
        }
        [HttpPatch("changeprofile")]
        [TypeFilter(typeof(IdVerifier))]
         public async Task<ActionResult> ChangeProfile(changeProfileDto docs)
        {

            var UserId = (int?)HttpContext.Items["UserId"];

            var currentUser = await db.Users
                .FirstOrDefaultAsync(user => user.Id == UserId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }


            string tokens = "";


            if (!string.IsNullOrWhiteSpace(docs.Name))
            {
                currentUser.Name = docs.Name;
            }

            if (!string.IsNullOrWhiteSpace(docs.Email))
            {
                currentUser.Email = docs.Email;
            }

            if (docs.MobileNumber.HasValue)
            {
                currentUser.MobileNumber = docs.MobileNumber.Value;
            }

            if (!string.IsNullOrWhiteSpace(docs.Address))
            {
                currentUser.Address = docs.Address;
            }

            if (!string.IsNullOrWhiteSpace(docs.ProfileImageUrl))
            {
                currentUser.ProfileImageUrl = docs.ProfileImageUrl;
            }

            await db.SaveChangesAsync();

            tokens = token.getToken(
                currentUser.Name,
                currentUser.role.ToUpperInvariant(),
                currentUser.Id.ToString()
            );

            return Ok(new { User = new { Name = currentUser.Name, role = currentUser.role }, token = tokens });
        }
    }
}