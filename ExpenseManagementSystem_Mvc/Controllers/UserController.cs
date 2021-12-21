using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExpenseManagementSystem_Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace ExpenseManagementSystem_Mvc.Controllers
{
    public class UserController : Controller
    {
        ExpenseApi api = new ExpenseApi();

      /*====================================LOGIN=======================================*/
        //displaying Login page.
        public ActionResult Login()
        {
            return View();
        }

        //Using Post api authenticating user and passing product details based on user id.
        [HttpPost]
        public IActionResult Login(LoginData user)
        {
            HttpClient client = api.Initial();
            var responseTask = client.PostAsJsonAsync<LoginData>("api/user/Login", user);
            responseTask.Wait();
            
            var result = responseTask.Result;
            int userId=0;string name=null;
            if (result.IsSuccessStatusCode)
            {
                List<UserData> userLogin = new List<UserData>();
                userLogin = result.Content.ReadAsAsync<List<UserData>>().Result;
                
                foreach(var item in userLogin)
                {
                    userId = item.User_Id;
                    name = item.Full_Name;
                    break;
                }
                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetString("UserName", name);
                return RedirectToAction("Profile", "User");
            }
           ModelState.AddModelError(string.Empty, "Please Enter Valid Credentials");
           return View(user);
        }

      /*====================================LOGOUT=======================================*/
        //displaying Login page.
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                HttpContext.Session.Remove("UserId");
                HttpContext.Session.Remove("ProductData");
                HttpContext.Session.Remove("UserName");
                return RedirectToAction("Login", "User");
            }
            catch(Exception e)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
                return NoContent();
            }
            
        }

        /*====================================SIGN UP========================================*/
        //To Show SignUp Page.
        public ActionResult SignUp()
        {
            ViewBag.message = null;
            //ViewBag.message = "Sign Up Successfully!";
            return View();
        }

        //Using Post Api To Add Data in DATABASE.
        [HttpPost]
        public IActionResult SignUp(UserData user)
        {
            HttpClient client = api.Initial();
            var PostTask = client.PostAsJsonAsync<UserData>("api/User/PostNewUser", user);
            var result = PostTask.Result;
            if (result.IsSuccessStatusCode)
            {
                ViewBag.message = "Sign Up Successfully!";
                ModelState.Clear();
                return View();
                //return RedirectToAction("Login", "User");
            }
            ModelState.AddModelError(string.Empty, "Server Error. Please Enter Valid Credentials");
            return View(user);
        }

      /*====================================PROFILE=========================================*/

        //Displaying Prodcut details based on login.
        public IActionResult Profile()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.name = userName;
            List<ProductData> products = GetProductList(userId);

            HttpContext.Session.SetString("ProductData", JsonConvert.SerializeObject(products));
            TempData["ProductData"] = JsonConvert.DeserializeObject<List<ProductData>>(HttpContext.Session.GetString("ProductData"));

            var productDetails = TempData["ProductData"] as List<ProductData>;
            return View(productDetails);
        }

        //Displaying User details based on login.
        public async Task<IActionResult> UserProfile()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            UserData users = new UserData();
            HttpClient client = api.Initial();
            HttpResponseMessage response = await client.GetAsync($"api/User/get-user/{userId}");

            if(response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<UserData>(result);
                List<UserData> userlist = new List<UserData>();
                userlist.Add(users);
                return View(userlist);
            }

            ModelState.AddModelError(string.Empty, "Invalid User");
            return NotFound();
        }

      /*================================ADD NEW PRODUCT=====================================*/

        //View for adding new product.
        public ActionResult AddItem()
        {
            ViewBag.id = HttpContext.Session.GetInt32("UserId");
            ViewBag.name = HttpContext.Session.GetString("UserName");
            return View();
        }

        //Adding new Product.
        [HttpPost]
        public IActionResult AddNewItem(ProductData product)
        {
            HttpClient client = api.Initial();
            var postTask = client.PostAsJsonAsync<ProductData>("api/User/PostNewItem", product);
            var result = postTask.Result;

            if(result.IsSuccessStatusCode)
                return RedirectToAction("Profile", "User");

            ModelState.AddModelError(string.Empty, "Please Enter Valid Credentials");
            return View();
        }

        /*=================================PRODUCT UPDATE=======================================*/

        //Used only for Update View.
        public async Task<IActionResult> Update(int id)
        {
            var product = new ProductData();
            HttpClient client = api.Initial();
            HttpResponseMessage responseTask = await client.GetAsync($"api/User/Get-Product/{id}");
            //var result = responseTask.Result;
            if (responseTask.IsSuccessStatusCode)
            {
                var result = responseTask.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<ProductData>(result);
            }
            return View(product);
        }

        //Put api to update the data into database.
        [HttpPost]
        public IActionResult Update(ProductData produts)
        {
            HttpClient client = api.Initial();
            var putTask = client.PutAsJsonAsync<ProductData>("api/User/update-product", produts);
            putTask.Wait();
            var result = putTask.Result;

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Profile", "User");
        
            ModelState.AddModelError(string.Empty, "Server Error");
            return View();
        }

        /*=================================USER UPDATE=======================================*/

        //Getting use data using GET api and displaying it.
        public async Task<IActionResult> UpdateUser(int id)
        {
            var users = new UserData();
            HttpClient client = api.Initial();
            HttpResponseMessage responseTask = await client.GetAsync($"api/User/get-user/{id}");
            //var result = responseTask.Result;
            if (responseTask.IsSuccessStatusCode)
            {
                var result = responseTask.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<UserData>(result);
            }
            return View(users);
        }

        //Put api to update the data into database.
        [HttpPost]
        public IActionResult UpdateUser(UserData user)
        {
            HttpClient client = api.Initial();
            var putTask = client.PutAsJsonAsync<UserData>("api/User/update-user", user);
            putTask.Wait();
            var result = putTask.Result;

            if (result.IsSuccessStatusCode)
                return RedirectToAction("UserProfile", "User");

            ModelState.AddModelError(string.Empty, "Server Error");
            return View();
        }

        /*=================================DELETE===========================================*/

        //Delete api to delete the particular data based on id.
        public ActionResult Delete(int id)
        {
            HttpClient client = api.Initial();
            var deleteTask = client.DeleteAsync($"api/User/{id}");
            var result = deleteTask.Result;

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Profile", "User");

            return NotFound();
        }

      /*================================GET PRODUCT LIST====================================*/

        //GETTING LIST OF PRODUCT BASED ON ID.
        [HttpGet]
        public List<ProductData> GetProductList(int userId)
        {
            HttpClient client = api.Initial();
            var responseTask = client.GetAsync($"api/user/product-list/{userId}");

            var result = responseTask.Result;

            List<ProductData> products = new List<ProductData>();
            if (result.IsSuccessStatusCode)
                products = result.Content.ReadAsAsync<List<ProductData>>().Result;

            return products;
        }

      /*===================================================================================*/



        // GET: User
        /* public async Task<ActionResult> Index(int userId)
         {
             HttpClient client = api.Initial();
             HttpResponseMessage response = await client.GetAsync($"api/User/product-list/{userId}");

             if(response.IsSuccessStatusCode)
             {
                 List<ProductData> products = new List<ProductData>();
                 var result = response.Content.ReadAsStringAsync().Result;
                 products = JsonConvert.DeserializeObject<List<ProductData>>(result);
                 return View(products);
             }
             ModelState.AddModelError(string.Empty, "Bad Request");
             return NotFound();
         }


         // POST: User/Create
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(IFormCollection collection)
         {
             try
             {
                 // TODO: Add insert logic here

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }
         }

         // GET: User/Edit/5


         // POST: User/Edit/5
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Edit(int id, IFormCollection collection)
         {
             try
             {
                 // TODO: Add update logic here

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }
         }

         // GET: User/Delete/5
         public ActionResult Delete(int id)
         {
             return View();
         }

         // POST: User/Delete/5
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Delete(int id, IFormCollection collection)
         {
             try
             {
                 // TODO: Add delete logic here

                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }
         }*/
    }
}