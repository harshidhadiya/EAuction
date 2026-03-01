using MACUTION.Data;
using MACUTION.Middleware.AddEndpointFilter;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "admin")]
    [ServiceFilter(typeof(VerifiedAdminCanVerifyFilter))]
    public class verificationController : ControllerBase
    {
        private MacutionDatabase db;
        public verificationController(MacutionDatabase db)
        {
            this.db = db;
        }

        [HttpGet("getallproduct")]
        public ActionResult getAllProduct() => GetAllUnverifiedProducts();

        [HttpGet("getallunverified")]
        public ActionResult GetAllUnverifiedProducts()
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
            var last = db.Products
                .Include(x => x.user)
                .Where(x => x.verifier == null || (x.verifier != null && !x.verifier.isverified))
                .OrderByDescending(x => x.creation_date)
                .Select(x => new allUnverifiedProduct_responce
                {
                    buydate = x.Buy_Date,
                    Id = x.Id,
                    productname = x.product_name,
                    ownerDetails = x.user != null ? new supportClass(x.user.Id, x.user.Name, x.user.Email, x.user.Address, x.user.ProfileImageUrl) : null
                })
                .ToList();
            if (last == null || last.Count == 0)
            {
                return NoContent();
            }
            return Ok(last);
        }

        /// Get all products verified by you (current verified admin)
        [HttpGet("getallverifiedbyyou")]
        public ActionResult GetAllVerifiedByYou()
        {
            var userIdObj = HttpContext.Items["verified_admin_user_id"] ?? HttpContext.Items["id"];
            if (userIdObj == null || !int.TryParse(userIdObj.ToString(), out var verifierUserId))
            {
                return BadRequest(new { status = "fail", message = "Could not determine verified admin identity." });
            }
            var list = db.Products
                .Include(x => x.user)
                .Include(x => x.verifier)
                .Where(x => x.verifier != null && x.verifier.verifier_id == verifierUserId && x.verifier.isverified)
                .OrderByDescending(x => x.verifier!.verificationDate)
                .Select(x => new VerifiedByYouProductResponse
                {
                    Id = x.Id,
                    productname = x.product_name,
                    buydate = x.Buy_Date,
                    ownerDetails = x.user != null ? new supportClass(x.user.Id, x.user.Name, x.user.Email, x.user.Address, x.user.ProfileImageUrl) : null,
                    verificationDate = x.verifier!.verificationDate,
                    verificationDescription = x.verifier.description
                })
                .ToList();
            if (list == null || list.Count == 0)
            {
                return NoContent();
            }
            return Ok(list);
        }

        /// Verify a product. Only verified admins can call this. Creates a Verifier record and links it to the product
        [HttpPost("verify/product/{productId:int}")]
        public ActionResult VerifyProduct(int productId, VerifyProductRequestDto? body = null)
        {
            var userIdObj = HttpContext.Items["verified_admin_user_id"] ?? HttpContext.Items["id"];
            if (userIdObj == null || !int.TryParse(userIdObj.ToString(), out var verifierUserId))
            {
                return BadRequest(new { status = "fail", message = "Could not determine verified admin identity." });
            }
            var product = db.Products.Include(p => p.verifier).FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound(new { status = "fail", message = $"Product with id {productId} not found." });
            }
            if (product.verifier != null && product.verifier.isverified)
            {
                return BadRequest(new { status = "fail", message = "Product is already verified." });
            }
            string? description = body?.description;
            if (product.verifier != null)
            {
                product.verifier.isverified = true;
                product.verifier.verifier_id = verifierUserId;
                product.verifier.verificationDate = DateTime.UtcNow;
                product.verifier.description = description ?? product.verifier.description;
                db.verifiers.Update(product.verifier);
                db.SaveChanges();
                return Ok(new { status = "success", message = "Product verified successfully.", productId, verifierId = product.verifier.Id });
            }
            var verifier = new Verifier
            {
                product_id = productId,
                verifier_id = verifierUserId,
                isverified = true,
                verificationDate = DateTime.UtcNow,
                description = description ?? null
            };
            db.verifiers.Add(verifier);
            db.SaveChanges();
            return Ok(new { status = "success", message = "Product verified successfully.", productId, verifierId = verifier.Id });
        }

        // unverify Product

        [HttpPost("unverify/product/{productId:int}")]
        public ActionResult unVerifyProduct(int productId, VerifyProductRequestDto? body = null)
        {
            var userIdObj = HttpContext.Items["verified_admin_user_id"] ?? HttpContext.Items["id"];
            if (userIdObj == null || !int.TryParse(userIdObj.ToString(), out var verifierUserId))
            {
                return BadRequest(new { status = "fail", message = "Could not determine verified admin identity." });
            }
            var product = db.Products.Include(p => p.verifier).FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return NotFound(new { status = "fail", message = $"Product with id {productId} not found." });
            }
            if (product.verifier == null || !product.verifier.isverified)
            {
                return BadRequest(new { status = "fail", message = "Product is already unverified." });
            }
            
            if (product.verifier.verifier_id != null && product.verifier.verifier_id!=verifierUserId)
            return Unauthorized(new{status="fail",message="Sorry but you have not right to unverified this product "});


            string? description = body?.description;
            product.verifier.isverified = false;
            product.verifier.verifier_id = verifierUserId;
            product.verifier.verificationDate = DateTime.UtcNow;
            product.verifier.description = description ?? product.verifier.description;
            db.verifiers.Update(product.verifier);
            db.SaveChanges();
            return Ok(new { status = "success", message = "Product verified successfully.", productId, verifierId = product.verifier.Id });

        }
    }
}