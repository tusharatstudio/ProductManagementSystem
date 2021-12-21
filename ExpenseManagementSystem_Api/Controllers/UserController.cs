using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManagementSystem_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementSystem_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private EMSDbContext context;

        public UserController(EMSDbContext _context)
        {
            context = _context;
        }

      /*================================GET API===========================================*/
        // GET: api/User/user-list  Returing list of users.
        [HttpGet("user-list")]
        public ActionResult<IEnumerable<User>> GetUserList()
        {
            return context.Users.ToList();
        }

        [HttpGet("get-user/{id}")]
        public User GetUser(int id)
        {
            var result = context.Users.FirstOrDefault(x => x.User_Id == id);
            return result;
        }

        // GET: api/User/Get-Product/1  Returing product object based on id.
        [HttpGet("Get-Product/{id}")]
        public Product GetProduct(int id)
        {
            var product = context.Products.FirstOrDefault(x => x.Product_Id == id);
            return product;
        }

        // GET: api/User/product-list/1 Returing list of products based on id.
        [HttpGet("product-list/{id}")]
        public ActionResult<IEnumerable<Product>> GetProductList(int id)
        {
            var res = from p in context.Products
                      join u in context.Users on p.User_Id equals u.User_Id
                      where u.User_Id == id
                      select p;
            return res.ToList();
        }

      /*=================================POST API============================================*/

        //POST: api/User/Login  for authenticating login credentials.
        [HttpPost]
        [Route("[action]")]
        public ActionResult<IEnumerable<User>> Login([FromBody] LoginData login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data.");

            int res1 = context.Users.Where(
                      x => x.Email_Id == login.Email_Id && x.Password == login.Password).Count();
            if (res1 > 0)
            {
                var user = from u in context.Users
                           where u.Email_Id == login.Email_Id && u.Password == login.Password
                           select u;//new { u.User_Id, u.Full_Name };
                return user.ToList();
            }
            else
                return NotFound();
        }

        //POST: api/User/PostNewUser(Signup) for Adding Details of user to Database.
        [HttpPost]
        [Route("[action]")]
        public IActionResult PostNewUser([FromBody] User user)
        {
            
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data.");

            context.Users.Add(user);
            context.SaveChanges();
            return Ok("Details Added Successfully!");
            
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult PostNewItem([FromBody] Product product)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid Data.");

            context.Products.Add(product);
            context.SaveChanges();            
            return Ok();
        }

        /*====================================PUT API============================================*/

        // PUT: api/User/5 udtating product details based on product id.
        [HttpPut("update-product")]
        public ActionResult PutProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }
            var ExistingProduct = context.Products.Where(x => x.Product_Id == product.Product_Id);
            if (ExistingProduct != null)
            {
                context.Products.Update(product);
                context.SaveChanges();
                return Ok();
               
            }
            else
                return NotFound();
        }

        [HttpPut("update-user")]
        public ActionResult PutUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Not a valid model");
            }
           context.Users.Update(user);
           if(context.SaveChanges()>0)
                return Ok();

           return NotFound();
        }

        /*===================================DELETE API============================================*/

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (id < 0)
                return BadRequest("Not a valid Student");

            var ExistingProduct = context.Products.Find(id);

            if (ExistingProduct != null)
            {
                context.Products.Remove(ExistingProduct);
                context.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }
    }
}
