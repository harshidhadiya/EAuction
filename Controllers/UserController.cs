using System.Diagnostics.SymbolStore;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using MACUTION.Model.ActualObj;
using MACUTION.Model.Dto;
using MACUTION.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MACUTION.Controllers
{ 
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController :ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private PasswordHasher<object> hash;
        public UserController(ILogger<UserController> logger,PasswordHasher<object> hash)
        {
            this._logger=logger;
            this.hash=hash;
        }
        [HttpPost("createUser")]
        public async Task<ActionResult> CreateUser(UserCreation user)
        {
            var ExistUser=Users.allUser.Where(x=>x.Name==user.Name).FirstOrDefault();
            if (ExistUser!=null)
            {
                return BadRequest("sorry on this name user already exist");
            }

            var createUser=new User(hash){Name=user.Name,role=user.role};
            Users.allUser.Add(createUser);
            var claims =new[] {new Claim("role",user.role),new Claim("Name",user.Name)};
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("harshidHADIYAHOWAREYOUDFSDFSDSDFGS"));
            var signature=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var tokens=new JwtSecurityToken(claims:claims,signingCredentials:signature,expires:DateTime.UtcNow.AddMinutes(5));
            var tokenstring=new JwtSecurityTokenHandler().WriteToken(tokens);
            return Created("/DATA",new {name=user.Name, role=user.role,token=tokenstring});
        }
        [HttpGet("getAllUser")]
        [Authorize]
        public ActionResult getAllUser()
        {
            
            var ExistUser=Users.allUser.Select(x=>new {x.Name,x.role}).ToArray();
            return Ok(ExistUser);
        }
    }
}