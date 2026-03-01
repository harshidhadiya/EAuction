using System.Security.Claims;
using MACUTION.Data;
using MACUTION.Middleware.AddEndpointFilter;
using MACUTION.Model.Dto;
using MACUTION.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Name;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class userController : ControllerBase
    {
        private readonly ILogger<userController> _logger;
        private readonly ItokenGeneration token;
        private PasswordHasher<object> hash; private readonly MacutionDatabase _db;

        public userController(
            ILogger<userController> logger,
            PasswordHasher<object> hash,
            ItokenGeneration token,
            MacutionDatabase db)
        {
            this._logger = logger;
            this.hash = hash;
            this.token = token;
            this._db = db;
        }
        [HttpPost("createUser")]
        public async Task<ActionResult> CreateUser(UserCreation user)
        {

            // Check if email already exists because it must be unique.
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == user.Email);

            if (existingUser != null)
            {
                return BadRequest("User already exists with this email");
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Address = user.Address,
                ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,
                role = user.role,
                createdAt = DateTime.UtcNow
            };

            var hashedPassword = hash.HashPassword(new object(), user.Password);
            newUser.setPassword(hashedPassword);

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            var generatedToken = token.getToken(
                newUser.Name,
                newUser.role.ToUpperInvariant(),
                newUser.Id.ToString()
            );

            return Created("/api/v1/user/getprofile", new { name = newUser.Name, role = newUser.role, token = generatedToken });
        }
        [HttpGet("getAllUser")]
        [Authorize(Policy = "user")]
        public async Task<ActionResult> getAllUser()
        {
            var users = await _db.Users
                .Select(x => new { x.Name, x.role })
                .ToListAsync();

            return Ok(new { ExistUser = users });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDto user)
        {
            var existUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(y => y.Email == user.Email);
            if (existUser == null)
            {
                return BadRequest(new { msg = "User Not Exist with this email" });
            }
            var verifyPass = hash.VerifyHashedPassword(new object(), existUser.Password, user.Password);
            if (verifyPass == PasswordVerificationResult.Failed)
            {
                return BadRequest(new { msg = "Incorrecte Password" });
            }
            if (user.Role != existUser.role)
            {
                return BadRequest(new { msg = "Role Din't Match" });
            }
            var generatedToken = token.getToken(existUser.Name, user.Role.ToUpperInvariant(), existUser.Id.ToString());
            return Ok(new { token = generatedToken, Name = existUser.Name, Id = existUser.Id });
        }
        [HttpPost("changepassword")]
        [Authorize]
        public async Task<ActionResult> changePassword(changePasswordDto changePass)
        {
            var Id = HttpContext.Items["id"];
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }


            if (!int.TryParse(Id.ToString(), out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }

            var currentUser = await _db.Users
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            var hashedPassword = hash.HashPassword(new object(), changePass.password);
            currentUser.setPassword(hashedPassword);

            await _db.SaveChangesAsync();

            return Ok(new { Id = currentUser.Id, Name = currentUser.Name, role = currentUser.role });
        }
        [HttpPatch("changeprofile")]
        [TypeFilter(typeof(IdVerifier))]
        [Authorize]
        public async Task<ActionResult> ChangeProfile(changeProfileDto docs)
        {

            var UserId = (int?)HttpContext.Items["UserId"];

            var currentUser = await _db.Users
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

            await _db.SaveChangesAsync();

            tokens = token.getToken(
                currentUser.Name,
                currentUser.role.ToUpperInvariant(),
                currentUser.Id.ToString()
            );

            return Ok(new { User = new { Name = currentUser.Name, role = currentUser.role }, token = tokens });
        }
        [HttpGet("getprofile")]
        [Authorize(Policy = "user")]
        [TypeFilter(typeof(IdVerifier))]
        public async Task<ActionResult> getProfile()
        {
            var userId = (int?)HttpContext.Items["UserId"];
            var currentUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            return Ok(new { Id = currentUser.Id, Name = currentUser.Name, role = currentUser.role });
        }

    }
}