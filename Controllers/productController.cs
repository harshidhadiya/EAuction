using MACUTION.Data;
using MACUTION.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MACUTION.Controllers
{
   [ApiController]
   [Route("api/v1/[controller]")]
   [Authorize(Policy ="user")]
    public class productController : ControllerBase
    {
        private readonly MacutionDatabase _db;

        public productController(MacutionDatabase db)
        {
            _db = db;
        }

    //   creation of the product  
     [HttpPost("addproduct")]
     public ActionResult createProduct(productDto product)
        {
            var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }

            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }

            var currentUser = _db.Users
                .FirstOrDefault(user => user.Id == userId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            var existingProduct = _db.Products
                .FirstOrDefault(x => x.product_name == product.Name && x.user_id == currentUser.Id);

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
            _db.SaveChanges();

            return Created("", new
            {
                name = createdproduct.product_name,
                date = createdproduct.Buy_Date,
                id = createdproduct.Id,
                userId = createdproduct.user_id
            });
        }
        [HttpGet("getproducts")]
        public ActionResult getAllProduct()
        {
             var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }

            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }

            var currentUser = _db.Users
                .FirstOrDefault(user => user.Id == userId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            var allProducts = _db.Products
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
                .ToArray();

            return Ok(new { product = allProducts });
        }
        [HttpDelete("deleteproduct/{id}")]
        public ActionResult deleteProduct(int id)
        {
            var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }

            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }

            var currentUser = _db.Users
                .FirstOrDefault(user => user.Id == userId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            var product = _db.Products
                .FirstOrDefault(product => product.user_id == currentUser.Id && product.Id == id);

            if (product == null)
            {
                return BadRequest("May Be This Product Doesn't exist or You are not ownere of this product");
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
            
            return Ok(new { message = $"Deleted {product.product_name} success fully" });
        }
        [HttpPatch("updateproduct/{id}")]
        public ActionResult updateProduct(int id,changeProductDto cp)
        {
             var Id = User.Claims.Where(x => x.Type == "ID").FirstOrDefault()?.Value;
            if (Id == null)
            {
                return BadRequest("Token Not Content Id ");
            }

            if (!int.TryParse(Id, out var userId))
            {
                return BadRequest("Token Id is not valid.");
            }

            var currentUser = _db.Users
                .FirstOrDefault(user => user.Id == userId);

            if (currentUser == null)
            {
                return BadRequest("Current Id relate User Not Exist");
            }

            var product = _db.Products
                .FirstOrDefault(product => product.user_id == currentUser.Id && product.Id == id);

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

            _db.SaveChanges();

            return Ok(new
            {
                message = "Your Updated Data",
                data = new
                {
                    name = product.product_name,
                    id = product.Id,
                    description = product.verifier != null ? product.verifier.description : null,
                    date = product.Buy_Date,
                    userid = product.user_id,
                    verified = product.verifier != null && product.verifier.isverified
                }
            });
        }
    }
}


// {
//     "nameOfProduct": "data",
//     "description": "datas",
//     "buyDate": "string",
//     "userId": "87784989d6b74f0091d96a7578ba4c51",
//     "productId": "065c2fa31db54ecd8fba9b215f73992b"
// }