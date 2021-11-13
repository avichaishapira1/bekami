using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using bekami.Data;
using bekami.Models;

namespace bekami.Controllers
{
    public class CartController : Controller
    {
        private readonly bekamiContext _context;

        public CartController(bekamiContext context)
        {
            _context = context;
        }

        

        //View your cart
        public async Task<IActionResult> Index()
        {
            var cart = GetShoppingCart();

            var itemsList = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product);
            foreach (var item in itemsList)
                if (item.Product.IsAvailable == false)
                {
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }

            ViewBag.Total = GetTotalPrice();
            return View(await _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product).ToListAsync());
        }


        // View Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetShoppingCart();
            if (GetNumOfItems() == 0)
                return RedirectToAction(nameof(Index));

            var itemsList = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product);
            foreach (var item in itemsList)
                if (item.Product.IsAvailable == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }

            return View();
        }


        [Authorize]
        public async Task<IActionResult> Summary()
        {
            var cart = GetShoppingCart();
            ViewBag.Total = GetTotalPrice();
            return View(await _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product).ToListAsync());
        }


        // POST: Create Orders
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Checkout([Bind("Id,Email,Address,ZipCode,PhoneNumber,CreditCardNum")] Order order)
        {
            var NumOfItems = GetNumOfItems();
            var cart = GetShoppingCart();
            if (NumOfItems == 0)
                return RedirectToAction(nameof(Index));


            var itemsList = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product);
            foreach (var item in itemsList)
                if (item.Product.IsAvailable == false || item.Product == null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }



            string email = User.Claims.FirstOrDefault(c => c.Type == "userEmail").Value;
            order.User = _context.User.FirstOrDefault(i => i.Email == email);

            order.Created = DateTime.Now;
            order.Total = GetTotalPrice();
            order.Status = OrderStatus.Processing;
            order.NumOfItems = NumOfItems;

            if (ModelState.IsValid)
            {
                _context.Add(order);

                var cartItems = _context.CartItem.Where(i => i.Cart == cart).Include(p => p.Product);
                foreach (CartItem item in cartItems)
                {
                    OrderProduct orderItem = new OrderProduct
                    {
                        Order = order,
                        Product = item.Product,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    };
                    _context.Add(orderItem);
                    _context.Remove(item);
                }
                _context.Remove(cart);
                _context.SaveChanges();
                return RedirectToAction("MyProducts", "Orders", order);
            }
            return View(order);
        }

        

        // Admin...
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Product.FirstOrDefault(i => i.ProductId == productId);
            if (product == null)
                return Json("Product Doesnt Exists!!!");

            if (product.IsAvailable == false)
                return Json("Sorry, Product is out of stock.");

            var cart = GetShoppingCart();

            var cartItem = _context.CartItem.Include(p => p.Product)
                .Include(p => p.Cart).FirstOrDefault(i => i.Cart == cart);
            if (cartItem == null)
            {
                cartItem = new CartItem { Product = product, UnitPrice = product.Price, Cart = cart };
                _context.CartItem.Add(cartItem);
                _context.SaveChanges();
            }

            
      
            cartItem.Quantity++;
            _context.SaveChanges();

            return View("../Products/Shop");

            
        }



        public IActionResult UpdateItemQuantity(int cartItemId, int quantity)
        {
            if (quantity > 5 || quantity < 0)
                return Json("Wrong Quantity");


            var cart = GetShoppingCart();

            var cartItem = _context.CartItem.Include(p => p.Product).FirstOrDefault(i => i.Cart.CartId == cart.CartId && i.ItemId == cartItemId);
            if (cartItem == null)
                return Json("NOT EXISTS!");

            if (quantity == 0)
            {
                _context.CartItem.Remove(cartItem);
                _context.SaveChanges();
                return Json(cartItem.Product.Name.ToString() + "Removed!");
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
                return Json(cartItem.Product.Name.ToString() + "Update!");
            }
        }


        public IActionResult EmptyCart()
        {
            var cart = GetShoppingCart();
            var cartItems = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId);
            foreach (var item in cartItems)
                _context.CartItem.Remove(item);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }









        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                                         GET TotalPrice and Number OF Items Methods
        ///                                                  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //returns Total price of all items
        public decimal GetTotalPrice()
        {
            var cart = GetShoppingCart();
            var cartItems = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId);
            decimal total = 0;
            foreach (var item in cartItems)
                total += item.UnitPrice * item.Quantity;

            return total;
        }

        //returns Num of items in the cart, used for counter and shoppingcartindex
        public int GetNumOfItems()
        {
            var cart = GetShoppingCart();
            var cartItems = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId);
            int numOfItems = 0;
            foreach (var item in cartItems)
                numOfItems += item.Quantity;

            return numOfItems;
        }







        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                                        GetShoppingCart Using Cookies
        ///                                                  
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///Returns customer ShoppingCart according to GUID(uniqueID) placed in cookieI, f don't have one: create one! 
        private Cart GetShoppingCart()
        {
            //retrive GUID from cookie and look in DB for the ShoppingCart
            string cartGUID = HttpContext.Request.Cookies["ShoppingCartCookie"];
            var customerShoppingCart = _context.Cart.FirstOrDefault(i => i.AccountSessionID == cartGUID);

            // if the shoppingcart GUID doesn't exist in the cookie or no GUID like that in DB -> generate a new shoppingcart (old 1 may be still in DB)
            if (cartGUID == null || customerShoppingCart == null)
            {
                //if no GUID in cookie -> create cookie
                if (cartGUID == null)
                {
                    cartGUID = Guid.NewGuid().ToString();
                    CookieOptions option = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
                    HttpContext.Response.Cookies.Append("ShoppingCartCookie", cartGUID, option); //seed cookie
                }

                //create ShoppingCart assosiated with the GUID
                customerShoppingCart = new Cart { AccountSessionID = cartGUID, Created = DateTime.Now, LastUpdate = DateTime.Now };
                _context.Cart.Add(customerShoppingCart);
            }
            customerShoppingCart.LastUpdate = DateTime.Now;
            _context.SaveChanges();
            return customerShoppingCart;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




    }

}

