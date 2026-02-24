using System.Diagnostics.SymbolStore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using MACUTION.Model.ActualObj;
using MACUTION.Model.Dto;
using MACUTION.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Name;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class userController : ControllerBase
    {
        private readonly ILogger<userController> _logger;
        private readonly ItokenGeneration token;
        private PasswordHasher<object> hash;
        public userController(ILogger<userController> logger, PasswordHasher<object> hash, ItokenGeneration token)
        {
            this._logger = logger;
            this.hash = hash;
            this.token = token;
        }
        [HttpPost("createUser")]
        public async Task<ActionResult> CreateUser(UserCreation user)
        {
            var ExistUser = Users.allUser.Where(x => x.Name == user.Name).FirstOrDefault();
            if (ExistUser != null)
            {
                return BadRequest("sorry on this name user already exist");
            }

            var createUser = new User(hash) { Name = user.Name, role = user.role };
            await createUser.setGenerateAndSetPassword(user.password);
            Users.allUser.Add(createUser);

            return Created("/DATA", new { name = user.Name, role = user.role, token = token.getToken(user.Name, user.role.ToUpperInvariant(), createUser.Id) });
        }
        [HttpGet("getAllUser")]
        [Authorize(Policy = "user")]
        public ActionResult getAllUser()
        {
           
            var ExistUser = Users.allUser.Select(x => new { x.Name, x.role }).ToArray();
            return Ok(new { ExistUser });
        }

        [HttpPost("login")]
        public ActionResult Login(UserCreation user)
        {
            var existUser = Users.allUser.Where(y => y.Name == user.Name).FirstOrDefault();
            if (existUser == null)
            {
                return BadRequest(new { msg = "User Not Exist on this name" });
            }
            var verifyPass = hash.VerifyHashedPassword(new object(), existUser.getPassword(), user.password);
            if (verifyPass == PasswordVerificationResult.Failed)
            {
                return BadRequest(new { msg = "Incorrecte Password" });
            }

            if (user.role != existUser.role)
            {
                return BadRequest(new { msg = "Role Din't Match" });
            }


            return Ok(new { token = token.getToken(user.Name, user.role.ToUpperInvariant(), existUser.Id), Name = user.Name, Id = existUser.Id });

        }
        [HttpPost("changepassword")]
        [Authorize]
        public async Task<ActionResult> changePassword(changePasswordDto changePass)
        {
            var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }
            Console.WriteLine($"Id= {Id}");


            var currentUser = Users.allUser.Where(user => user.Id == Id).FirstOrDefault();
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            await currentUser.setGenerateAndSetPassword(changePass.password);
            return Ok(currentUser);
        }
        [HttpPatch("changeprofile")]
        [Authorize]
        public ActionResult ChangeProfile(changeProfileDto docs)
        {

            var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }

            var currentUser = Users.allUser.Where(user => user.Id == Id).FirstOrDefault();
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            
            string tokens ="";

           
            if (docs.Name != "")
            {
                currentUser.Name = docs.Name;
            }
                tokens=token.getToken(currentUser.Name,currentUser.role,currentUser.Id);
            return Ok(new {User=new {Name=currentUser.Name,role=currentUser.role},token=tokens});
        }
        [HttpGet("getprofile")]
        [Authorize(Policy = "user")]
        public ActionResult getProfile()
        {
            Console.WriteLine("enered");
            var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;

            var currentUser = Users.allUser.Where(user => user.Id == Id).FirstOrDefault();
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            
            return Ok(new{Id=currentUser.Id,Name=currentUser.Name,role=currentUser.role});
        }

    }
}