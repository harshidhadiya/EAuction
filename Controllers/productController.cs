using MACUTION.Data;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "user")]
    public class productController : ControllerBase
    {
        private readonly MacutionDatabase _db;

        public productController(MacutionDatabase db)
        {
            _db = db;
        }

        [HttpPost("addproduct")]
        public async Task<ActionResult> createProduct(productDto product)
        {
            var Id = User.Claims.FirstOrDefault(x => x.Type == "ID")?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }
            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }
            var currentUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            var existingProduct = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.product_name == product.Name && x.user_id == currentUser.Id);
            if (existingProduct != null)
            {
                return BadRequest("You keep in the mind that you can't store the same name products okay ");
            }
            if (!DateTime.TryParse(product.date, out var buyDate))
            {
                return BadRequest("Buy date is not in correct format.");
            }
            var createdproduct = new Product
            {
                product_name = product.Name,
                Buy_Date = buyDate,
                user_id = currentUser.Id,
                creation_date = DateTime.UtcNow
            };
            _db.Products.Add(createdproduct);
            await _db.SaveChangesAsync();
            return Created("", new
            {
                name = createdproduct.product_name,
                date = createdproduct.Buy_Date,
                id = createdproduct.Id,
                userId = createdproduct.user_id
            });
        }

        [HttpGet("getproducts")]
        public async Task<ActionResult> getAllProduct()
        {
            var Id = User.Claims.FirstOrDefault(x => x.Type == "ID")?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }
            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }
            var currentUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            var allProducts = await _db.Products
                .AsNoTracking()
                .Include(p => p.verifier)
                .Where(p => p.user_id == currentUser.Id)
                .Select(x => new
                {
                    Name = x.product_name,
                    date = x.Buy_Date,
                    verified = x.verifier != null && x.verifier.isverified,
                    id = x.Id,
                    user = x.user_id,
                    desc = x.verifier != null ? x.verifier.description : null
                })
                .ToArrayAsync();
            return Ok(new { product = allProducts });
        }

        [HttpDelete("deleteproduct/{id}")]
        public async Task<ActionResult> deleteProduct(int id)
        {
            var Id = User.Claims.FirstOrDefault(x => x.Type == "ID")?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }
            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }
            var currentUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            var product = await _db.Products.FirstOrDefaultAsync(p => p.user_id == currentUser.Id && p.Id == id);
            if (product == null)
            {
                return BadRequest("May Be This Product Doesn't exist or You are not ownere of this product");
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Deleted {product.product_name} success fully" });
        }

        [HttpPatch("updateproduct/{id}")]
        public async Task<ActionResult> updateProduct(int id, changeProductDto cp)
        {
            var Id = User.Claims.FirstOrDefault(x => x.Type == "ID")?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }
            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }
            var currentUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == userId);
            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }
            var product = await _db.Products.Include(p => p.verifier).FirstOrDefaultAsync(p => p.user_id == currentUser.Id && p.Id == id);
            if (product == null)
            {
                return BadRequest("May Be This Product Doesn't exist or You are not ownere of this product");
            }
            if (cp.BuyDate != null)
            {
                if (!DateTime.TryParse(cp.BuyDate, out var newBuyDate))
                {
                    return BadRequest("Buy date is not in correct format.");
                }
                product.Buy_Date = newBuyDate;
            }
            if (cp.NameOfProduct != null)
            {
                product.product_name = cp.NameOfProduct;
            }
            await _db.SaveChangesAsync();
            return Ok(new
            {
                message = "Your Updated Data",
                data = new
                {
                    name = product.product_name,
                    id = product.Id,
                    description = product.verifier?.description,
                    date = product.Buy_Date,
                    userid = product.user_id,
                    verified = product.verifier != null && product.verifier.isverified
                }
            });
        }
    }
}
