using AutoMapper;
using MACUTION.Data;
using MACUTION.Middleware.AddEndpointFilter;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Authorization;
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
        private readonly MacutionDatabase db;
        private readonly ItokenGeneration token;
        private readonly PasswordHasher<object> hash;
        private
         readonly IMapper mapper;

        public adminController(MacutionDatabase db, ItokenGeneration itoken,IMapper map)
        {
            this.db = db;
            this.hash = new PasswordHasher<object>();
            this.token = itoken;
            this.mapper=map;
        }

        [HttpPost("request/signup")]
        public async Task<ActionResult> requestAdminSignup(signupRequestDto obj)
        {
            dynamic response;
            try
            {
                var data = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == obj.Email);
                if (data != null && data.role == "USER")
                {
                    return BadRequest(new { status = "fail", message = "You have already register as the user" });
                }
                if (data != null)
                {
                    return BadRequest(new { status = "fail", message = "You have already register try to login" });
                }
                string hashedPassword = hash.HashPassword(new object(), obj.Password);
                var user = db.Users.Add(new User { Address = obj.Address, createdAt = DateTime.Now, Email = obj.Email, MobileNumber = obj.MobileNumber, Name = obj.Name, Password = hashedPassword, role = obj.role, ProfileImageUrl = obj.ProfileImageUrl ?? "" });
                await db.SaveChangesAsync();
                var currentuser = user.Entity;
                if (currentuser == null)
                {
                    return BadRequest(new { status = "fail", message = "Try to again sign up server error " });
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
        public async Task<ActionResult> login(loginRequestDto obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "success", message = "add all required fields" });
            }
            var error = new List<string>();
            if (obj.email == "") error.Add("Your Mail Shouldn't be empty");
            if (obj.password == "") error.Add("Your password require here authenticate to you");
            if (obj.role == "") error.Add("Your Role shouldn't be empty");
            if (obj.role == "USER") error.Add("You have to require login as the admin");
            if (error.Count != 0)
            {
                return BadRequest(new { status = "fail", message = string.Join(", ", error) });
            }
            var data = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == obj.email);
            if (data == null || data.role == "USER")
            {
                return BadRequest(new { status = "fail", message = "Sorry But Your mail Doesn't exist Or You Already Stored As user role" });
            }
            var responce = mapper.Map<loginResponceDto>(data);
            return Ok(responce);
        }

        [HttpPatch("changeprofile")]
        [TypeFilter(typeof(IdVerifier))]
        public async Task<ActionResult> ChangeProfile(changeProfileDto docs)
        {
            var UserId = (int?)HttpContext.Items["UserId"];
            var currentUser = await db.Users.FirstOrDefaultAsync(user => user.Id == UserId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            if (!string.IsNullOrWhiteSpace(docs.Name)) currentUser.Name = docs.Name;
            if (!string.IsNullOrWhiteSpace(docs.Email)) currentUser.Email = docs.Email;
            if (docs.MobileNumber.HasValue) currentUser.MobileNumber = docs.MobileNumber.Value;
            if (!string.IsNullOrWhiteSpace(docs.Address)) currentUser.Address = docs.Address;
            if (!string.IsNullOrWhiteSpace(docs.ProfileImageUrl)) currentUser.ProfileImageUrl = docs.ProfileImageUrl;
            await db.SaveChangesAsync();
            var tokens = token.getToken(currentUser.Name, currentUser.role.ToUpperInvariant(), currentUser.Id.ToString());
            return Ok(new { User = new { Name = currentUser.Name, role = currentUser.role }, token = tokens });
        }

      
        [HttpGet("acceptrequest/{id:int}")]
        [Authorize(Policy = "admin")]
        [TypeFilter(typeof(acceptServiceVerifier))]
        public async Task<ActionResult> acceptHisAdmin(int id, string access = "false")
        {
            int id_of_current_admin = (int)HttpContext.Items["UserId"];
            var users = (IEnumerable<User>?)HttpContext.Items["Users"];
            var adding_user = users?.FirstOrDefault(x => x.Id == id);
            if (adding_user == null)
            {
                return NotFound(new { status = "failed", message = $"sorry we are not able to find this {id} admin request" });
            }
            if (adding_user.role == "USER")
            {
                return BadRequest(new { status = "fail", message = $"current user id {adding_user.Id} is sign up as the user role" });
            }
            if (id_of_current_admin == id) return BadRequest(new { status = "fail", message = $"You trying To give own id access" });
            if (id == 1) return BadRequest("Development purpose you can't give this person any access okay");
            if (adding_user.request != null)
            {
                var verication_data = adding_user.request;
                if (verication_data.verified_admin == true)
                {
                    var admin_datas = users?.FirstOrDefault(x => x.Id == id_of_current_admin);
                    if (admin_datas?.given_access != null)
                    {
                        var given_access_current_user = admin_datas.given_access?.FirstOrDefault(x => x.request_person_id == id);
                        if (given_access_current_user != null)
                        {
                            if (adding_user.request.verified_admin == false || (adding_user.right_to_add == false && access.ToUpperInvariant() == "TRUE"))
                            {
                                adding_user.request.verified_admin = true;
                                adding_user.right_to_add = access.ToUpperInvariant() != "FALSE";
                                return Ok(new acceptResponceDto { status = "success", message = $"You successfully give Access to the current user{id}", data = new { Name = adding_user.Name, id = adding_user.Id, Url = adding_user.ProfileImageUrl ?? "", is_Authentic_add_User = adding_user.right_to_add } });
                            }
                            return Ok(new { status = "fail", message = $"You already given access to current_id {id}" });
                        }
                    }
                    return BadRequest(new { status = "fail", message = "Access given by the another user" });
                }
            }
            var verfired = new Request_admin { request_person_id = id, give_access_person_id = id_of_current_admin, verified_admin = true };
            db.request_Admins.Add(verfired);
            if (access.ToUpperInvariant() == "TRUE")
            {
                adding_user.right_to_add = true;
            }
            await db.SaveChangesAsync();
            return Ok(new acceptResponceDto { status = "success", message = $"You successfully give Access to the current user{id}", data = new { Name = adding_user.Name, id = adding_user.Id, Url = adding_user.ProfileImageUrl ?? "", is_Authentic_add_User = adding_user.right_to_add } });
        }

        [HttpPost("restrictaccess/{id:int}")]
        [TypeFilter(typeof(acceptServiceVerifier))]
        public async Task<ActionResult> giveAction(int id, restrictionResponceDto restriction)
        {
            int id_of_current_admin = (int)HttpContext.Items["UserId"];
            var users = (IEnumerable<User>?)HttpContext.Items["Users"];
            var current = users?.FirstOrDefault(x => x.Id == id);
            if (current == null) return BadRequest(new { status = "fail", message = $"sorry but Your Given Id doesn't exist {id}" });
            if (current.role == "USER") return BadRequest(new { status = "fail", message = $"You Are Trying To updates on id: {id} which is having the USER role" });
            if (current.request == null) return BadRequest(new { status = "fail", message = $"This Id having already restriction's {id}" });
            if (current.Id == id_of_current_admin) return BadRequest(new { status = "fail", message = $"You Trying To Your own Restriction {id}" });
            if (current.request.give_access_person_id != id_of_current_admin)
            {
                var given_access_user = users?.Where(x => x.Id == current.request.give_access_person_id).Select(x => new { name = x.Name, mobileNo = x.MobileNumber, email = x.Email }).FirstOrDefault();
                return BadRequest(new { status = "fail", message = "You are not give the access of this id that's why we are not accepting it", data = given_access_user });
            }
            if (restriction.delete_verification == true)
            {
                current.request.give_access_person_id = id_of_current_admin;
                current.request.verified_admin = false;
                current.right_to_add = false;
            }
            if (restriction.add_to_antoher == true)
                current.right_to_add = false;
            await db.SaveChangesAsync();
            return Ok(new { status = "success", message = $"This Id {id} restriction's done on this " });
        }

        [HttpGet("getalladmins")]
        [Authorize(Policy = "admin")]
        public async Task<ActionResult> getuser()
        {
            var id = HttpContext.Items["id"];
            if (id == null)
                return BadRequest(new { status = "fail", message = "sorry maybe you are unauthorized we are not able to find you" });
            if (!int.TryParse(id.ToString(), out var userId))
                return BadRequest(new { status = "fail", message = "Your Id in Token Is Not valid" });
            var data = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (data == null)
                return BadRequest(new { status = "fail", message = "You are Not Exist in the database" });
            var users = await db.Users.AsNoTracking().Include(x => x.request).Where(x => x.role == "ADMIN").Select(x => new
            {
                id = x.Id,
                Name = x.Name,
                email = x.Email,
                has_right_to_add = x.right_to_add,
                verified = x.request != null && x.request.verified_admin,
                mobile_no = x.MobileNumber,
                address = x.Address,
                imageurl = x.ProfileImageUrl,
                role = x.role,
                createdAt = x.createdAt
            }).OrderByDescending(x => x.createdAt).ToListAsync();
            if (users == null || users.Count == 0) return NoContent();
            return Ok(users);
        }

        [HttpGet("getadmin/{ids:int}")]
        [Authorize(Policy = "admin")]
        public async Task<ActionResult> getSpecificUser(int ids)
        {
            var id = HttpContext.Items["id"];
            if (id == null)
                return BadRequest(new { status = "fail", message = "sorry maybe you are unauthorized we are not able to find you" });
            if (!int.TryParse(id.ToString(), out var userId))
                return BadRequest(new { status = "fail", message = "Your Id in Token Is Not valid" });
            var data = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (data == null)
                return BadRequest(new { status = "fail", message = "You are Not Exist in the database" });
            var user = await db.Users.AsNoTracking().Where(x => x.role == "ADMIN" && x.Id == ids).Select(x => new
            {
                id = x.Id,
                Name = x.Name,
                email = x.Email,
                has_right_to_add = x.right_to_add,
                verified = x.request != null && x.request.verified_admin,
                mobile_no = x.MobileNumber,
                address = x.Address,
                imageurl = x.ProfileImageUrl,
                role = x.role,
                createdAt = x.createdAt
            }).FirstOrDefaultAsync();
            if (user == null) return NoContent();
            return Ok(user);
        }

        [HttpGet("verifiedbyme")]
        [Authorize(Policy = "admin")]
        [TypeFilter(typeof(getVerifiedUnverified))]
        public async Task<ActionResult> getVerifiedByme()
        {
            var id = int.Parse(HttpContext.Items["id"]!.ToString()!);
            var last = await db.request_Admins.AsNoTracking().Include(x => x.requet_person).Where(x => x.verified_admin && x.give_access_person_id != null && x.give_access_person_id == id).Select(x => new
            {
                id = x.requet_person.Id,
                Name = x.requet_person.Name,
                email = x.requet_person.Email,
                has_right_to_add = x.requet_person.right_to_add,
                verified = true,
                give_access_person_id = x.give_access_person_id,
                mobile_no = x.requet_person.MobileNumber,
                address = x.requet_person.Address,
                imageurl = x.requet_person.ProfileImageUrl,
                role = x.requet_person.role,
                createdAt = x.requet_person.createdAt
            }).OrderByDescending(x => x.createdAt).ToListAsync();
            if (last == null || last.Count == 0) return NoContent();
            return Ok(last);
        }

        [HttpGet("unverifiedadmin")]
        [Authorize(Policy = "admin")]
        [TypeFilter(typeof(getVerifiedUnverified))]
        public async Task<ActionResult> getUnverifiedByme()
        {
            var id = int.Parse(HttpContext.Items["id"]!.ToString()!);
            var last = await db.Users.AsNoTracking().Include(x => x.request).Where(x => (x.request == null || x.request.verified_admin == false) && x.right_to_add == false).Select(x => new
            {
                x.Id,
                x.Name,
                x.Address,
                x.createdAt,
                x.MobileNumber,
                x.role,
                profilepic = x.ProfileImageUrl,
                has_right_to_add = x.right_to_add
            }).OrderByDescending(x => x.createdAt).ToListAsync();
            if (last == null || last.Count == 0) return NoContent();
            return Ok(last);
        }
    }
}
