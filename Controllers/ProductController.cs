using System.Reflection.Metadata.Ecma335;
using MACUTION.Model;
using MACUTION.Model.Dto;
using MACUTION.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MACUTION.Controllers
{
   [ApiController]
   [Route("api/v1/[controller]")]
   [Authorize(Policy ="user")]
    public class productController : ControllerBase
    {
    //   creation of the product  
     [HttpPost("addproduct")]
     public ActionResult createProduct(productDto product)
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
            var products= Products.allProduct.Where(x=>x.NameOfProduct==product.Name).Select(x=>x.NameOfProduct).FirstOrDefault();
            if (products != null)
            {
                return BadRequest("You keep in the mind that you can't store the same name products okay ");
            }
            var createdproduct=new Product(product.Name,product.date,currentUser.Id,product.Description);

            Products.allProduct.Add(createdproduct);
            return Created("",createdproduct);
        }
        [HttpGet("getproducts")]
        public ActionResult getAllProduct()
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
            var AllProduct=Products.allProduct.Where(product=>product.UserId==Id).Select(x=>new {Name=x.NameOfProduct,date=x.BuyDate,verified=x.verified.isVerified,id=x.ProductId,user=x.UserId,desc=x.Description}).ToArray();
            
          
            return Ok(new {product=AllProduct});
        }
        [HttpDelete("deleteproduct/{id}")]
        public ActionResult deleteProduct(string id)
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
            var product=Products.allProduct.Where(product=>product.UserId==Id && product.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return BadRequest("May Be This Product Doesn't exist or You are not ownere of this product");
            }

            Products.allProduct.Remove(product);
            
            return Ok(new {message=$"Deleted {product.NameOfProduct} success fully"});
        }
        [HttpPatch("updateproduct/{id}")]
        public ActionResult updateProduct(string id,changeProductDto cp)
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
            var product=Products.allProduct.Where(product=>product.UserId==Id && product.ProductId == id).FirstOrDefault();
            if (product == null)
            {
                return BadRequest("May Be This Product Doesn't exist or You are not ownere of this product");
            }
            if (cp.BuyDate != null)
            {
                product.changeDate(cp.BuyDate);

            }
            if (cp.Description!=null)
            {
                product.changeDescription(cp.Description);
            }
            if (cp.NameOfProduct != null)
            {
                product.changeName(cp.NameOfProduct);
            }
            return Ok(new {message="Your Updated Data",data=new{name=product.NameOfProduct,id=product.ProductId,descripiton=product.Description,date=product.BuyDate,userid=product.UserId,verified=product.verified.isVerified}});
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