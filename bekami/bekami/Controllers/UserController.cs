using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using bekami.Data;
using bekami.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

//TBH

namespace bekami.Controllers
{
    public class UserController : Controller
    {
        private readonly bekamiContext _context;

        public UserController(bekamiContext context)
        {
            _context = context;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                                           Admin Mangement:
        ///                                                  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            return View(await _context.User.ToListAsync());
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(Role)));
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Created,Role")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    //we may change to Role so we need to update the Claims
                    var foundUser = _context.User.FirstOrDefault(u => u.UserId == id);
                    CookieAuthentication(foundUser);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Admin));
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);

            //delete his reviews & orders
            _context.OrderProduct.RemoveRange(_context.OrderProduct.Where(p => p.Order.AssociatedUser == user));
            _context.Order.RemoveRange(_context.Order.Where(p => p.AssociatedUser == user));

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }



        //Filter for Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter(string term)
        {
            List<User> users;

            if (term != null)
                users = await _context.User.Where(c => c.UserId.ToString().Contains(term) || c.FirstName.Contains(term) ||
                                                       c.LastName.Contains(term) || c.Email.Contains(term)).ToListAsync();
            else
                users = await _context.User.ToListAsync();


            var query = from user in users
                        select new
                        {
                            id = user.UserId,
                            firstname = user.FirstName,
                            lastname = user.LastName,
                            email = user.Email,
                            role = user.Role.ToString(),
                            registrationdate = user.Created.ToShortDateString(),
                        };

            return Json(query);
        }


        private bool AccountExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
        //<--------------------------------------------END: ADMIN Mangement-------------------------------------------->







        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                              Account Registration, Login, Logout, Authentication by Cookies:
        ///                                                  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //<--------------------------------------------Account Registration-------------------------------------------->
        ///////////////////////////
        // GET: Accounts/Register
        ///////////////////////////
       // [Route("user/register")]
        public IActionResult Register()
        {
            //if already logged in
            if (User.Claims.FirstOrDefault(c => c.Type == "UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        ///////////////////////////
        // POST: Accounts/Register
        ///////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword")] User user)
        {
            //Setup UserType to Customer, Registration Date.
            user.Role = Role.Customer;
            user.Created = DateTime.Now;


            //(Registration Field, Error Message)
            //check if Email address already exists.
            if (_context.User.Any(i => i.Email == user.Email))
                ModelState.AddModelError("Email", "Email address already registered to another account.");
            


            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                CookieAuthentication(user);
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
        //<---------------------------------------------------END------------------------------------------------------>





        //<--------------------------------------------Account Login & Cookie Auth----------------------------------------->
        ///////////////////////////
        // GET: Accounts/Login
        ///////////////////////////
        public IActionResult Login()
        {
            //if already logged in
            if (User.Claims.FirstOrDefault(c => c.Type == "UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        ///////////////////////////
        // POST: Login = Check if user exsits and then create a CookieAuth D.M
        ///////////////////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.User.FirstOrDefault(i => i.Email == email && i.Password == password);

            if (user != null)
            {
                CookieAuthentication(user);
                return RedirectToAction("Index", "Home");
            }

            //need to create error message if login failed
            return View();
        }


        /////////////////////////////////////////////////
        // Private Method: Accounts Cookie Authentication
        /////////////////////////////////////////////////
        //Session configuration[10Min] located in Startup.cs
        private async void CookieAuthentication(User user)
        {
            //info you can use when using[Authorized], ex.[Authorize (Roles = "Admin")].
            var claims = new List<Claim>
                {
                    new Claim("Name", user.FirstName),
                    new Claim("userEmail", user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                };

            //startup.cs configuration
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //options
            var authProperties = new AuthenticationProperties
            {
                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
        //<---------------------------------------------------END------------------------------------------------------>





        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                                                Customer Area:
        ///                                                  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Account Details for specific Id taken from the Claims 
        public async Task<IActionResult> MyAccount()
        {
            ////not logged in => so login!
            if (User.Claims.FirstOrDefault(c => c.Type == "userEmail") == null)
                return RedirectToAction("Login");

            var email = User.Claims.FirstOrDefault(c => c.Type == "userEmail").Value;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);

            return View(user);
        }

        public async Task<IActionResult> EditMyAccount()
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "UserEmail") == null) //not logged in
                return RedirectToAction("Login");

            var email = User.Claims.FirstOrDefault(c => c.Type == "UserEmail").Value;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMyAccount(string Email, [Bind("Id,Username,FirstName,LastName,Email,Password,ConfirmPassword,CreditCardNum,RegistrationDate")] User user)
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "UserEmail") == null) //not logged in
                return RedirectToAction("Login");

            //Details taken from Claims compared to the one with the post request to auth
            var email = User.Claims.FirstOrDefault(c => c.Type == "UserEmail").Value;
            if (Email != email)
            {
                return RedirectToAction("Login");
            }



            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyAccount));
            }
            return RedirectToAction("MyAccount");
        }
        //<---------------------------------------------END: Customer Area--------------------------------------------->
    }




}
