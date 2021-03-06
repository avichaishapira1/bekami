using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bekami.Data;
using bekami.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace bekami.Controllers
{
    public class OrdersController : Controller
    {
        private readonly bekamiContext _context;

        public OrdersController(bekamiContext context)
        {
            _context = context;
        }
        
       
        public async Task<IActionResult> MyOrders()
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "userEmail") == null)
                return RedirectToAction("Login", "User");

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "userEmail").Value;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == userEmail);

            return View(await _context.Order.Where(i => i.User == user).ToListAsync());
        }

        public async Task<IActionResult> MyProducts(int? id)
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "userEmail") == null)
                return RedirectToAction("Login", "User");

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == "userEmail").Value;
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == userEmail);


            if (id == null)
                return NotFound();


            var order = await _context.Order.Include(o => o.ProductsOrdered).FirstOrDefaultAsync(m => (m.Id == id) && (m.User == user));

            if (order == null)
                return NotFound();

            var orderProducts = await _context.OrderProduct.Where(i => i.OrderId == order.Id).Include(i => i.Product).ToListAsync();

            if (orderProducts.Count == 0)
            {
                return NotFound();
            }
            
            return View(orderProducts);

        }



       //admin...

        
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var order = await _context.Order.Include(o => o.User)
                .Include(o => o.ProductsOrdered).ToListAsync();
            return View(order);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            //return View(order);
            return View(await _context.OrderProduct.Where(i => i.Order == order)
                .Include(c => c.Product).Include(c => c.Order.User).ToListAsync());
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Status = new SelectList(Enum.GetValues(typeof(OrderStatus)));
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,Total,Status,FirstName,LastName,Email,NumOfItems")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), order);
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            var orderDetails = _context.OrderProduct.Where(i => i.Order.Id == id);

            foreach (var item in orderDetails)
                _context.OrderProduct.Remove(item);

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }




        // Filter for Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter(string term)
        {
            List<Order> orders;

            if (term != null)
                orders = await _context.Order.Where(c => c.Id.ToString().Contains(term) ||
                                                         c.Total.ToString().Contains(term) || 
                                                         c.NumOfItems.ToString().Contains(term)).ToListAsync();
            else
                orders = await _context.Order.ToListAsync();


            var users = await _context.User.ToListAsync();
            var query = from order in orders
                        join user in users
                        on order.User.UserId equals user.UserId
                        select new
                        {
                            id = order.Id,
                            orderdate = order.Created.ToShortDateString(),
                            totalprice = order.Total,
                            numofitems = order.NumOfItems,
                            status = order.Status.ToString(),
                            email = user.Email,
                        };

            return Json(query);
        }
        //-----------------------------------------------------------------------------------------------------------------//
        //statistics
        public IActionResult Statistics()
        {
            return View();
        }

        public async Task<IActionResult> Usersinmonth()
        {
            var Usersinmonth = await _context.User.GroupBy(x => new { month = x.Created.Month})
                .Select(x => new { Created = x.Key.month, Users = x.Count() }).ToListAsync();
            return Json(Usersinmonth);
        }

        public async Task<IActionResult> Ordersinmonth()
        {
            var Ordersinmonth = await _context.Order.GroupBy(x => new { month = x.Created.Month })
                .Select(x => new { Created = x.Key.month, Orders = x.Count() }).ToListAsync();
            return Json(Ordersinmonth);
        }
    }
}
