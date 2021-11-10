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

            //if entering the cart and one items became out of stock , we implement here and not in ProductsController beacuase product can be in out of stock and then back, we dont want to delete for good
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
            //if u try to checkout with 0 items
            var cart = GetShoppingCart();
            if (GetNumOfItems() == 0)
                return RedirectToAction(nameof(Index));

            //if entering the checkout and one items became out of stock
            var itemsList = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product);
            foreach (var item in itemsList)
                if (item.Product.IsAvailable == false)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }

            return View();
        }


        //return table summary[html] of the shoppingcart(used in checkout) , because we using Order, cartItem, ShoppingCart controllers x3 and 1 view!
        [Authorize]
        public async Task<IActionResult> Table()
        {
            var cart = GetShoppingCart();
            ViewBag.Total = GetTotalPrice();
            return View(await _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product).ToListAsync());
        }


        // POST: Create Orders
        //Create an Order and OredDetails (copy the shoppingcart + get adress from user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Checkout([Bind("Id,Email,Address,ZipCode,PhoneNumber,CreditCardNum")] Order order)
        {
            //if u try to checkout with 0 items
            var NumOfItems = GetNumOfItems();
            var cart = GetShoppingCart();
            if (NumOfItems == 0)
                return RedirectToAction(nameof(Index));


            //if one product is out of stock or deleted let view the cart again
            var itemsList = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId).Include(c => c.Product);
            foreach (var item in itemsList)
                if (item.Product.IsAvailable == false || item.Product == null)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }



            string email = User.Claims.FirstOrDefault(c => c.Type == "userEmail").Value;
            order.UserId = _context.User.FirstOrDefault(i => i.Email == email);

            order.Created = DateTime.Now;
            order.Total = GetTotalPrice();
            order.Status = OrderStatus.Processing;
            order.NumOfItems = NumOfItems;

            if (ModelState.IsValid)
            {
                _context.Add(order);

                //coping CartItem into OrderProduct
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////









        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        ///                                                    ADD, Update, Empty, Cart Items
        ///*Mehods Should Be In Sync!!!                                                    
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        //Get [Product ID] and add it as CartItem to ShoppingCart, if already there ->Quantity++  (Limit to 5 items from the same product!)
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Product.FirstOrDefault(i => i.ProductId == productId);
            if (product == null)
                return Json("Product Doesnt Exists!!!");

            if (product.IsAvailable == false)
                return Json("Sorry, Product is out of stock.");

            var cart = GetShoppingCart();

            //checking if ShoppingCart item for this product exists, If no => create CartItem and place inside ShoppingCart
            var cartItem = _context.CartItem.Include(p => p.Product)
                .Include(p => p.Cart).FirstOrDefault(i => i.Cart == cart);
            if (cartItem == null)
            {
                //if reaced here: Products exists, CartItem doesnt => Create CartItem for this product!
                cartItem = new CartItem { Product = product, UnitPrice = product.Price, Cart = cart };
                _context.CartItem.Add(cartItem);
                _context.SaveChanges();
            }



            //If reached here: The CartItem realted to the product is already inside the cart => update Quantity, MAX TO 5!
            if (cartItem.Quantity + 1 > 5)
            {

                return Json("Sorry, we cannot add another " + cartItem.Product.Name + " to your cart as you've already added the maximum amount of 5");
            }
            else
            {
                cartItem.Quantity++;
                _context.SaveChanges();

                return Json(cartItem.Product.Name + " added to your cart");

            }
        }



        //Get [CartItem ID] and add update its quantity, quantity per product for customer is 5!
        public IActionResult UpdateItemQuantity(int cartItemId, int quantity)
        {
            if (quantity > 5 || quantity < 0)
                return Json("Wrong Quantity");


            var cart = GetShoppingCart();

            //checking if ShoppingCart item exists and its inside the cart.
            var cartItem = _context.CartItem.Include(p => p.Product).FirstOrDefault(i => i.Cart.CartId == cart.CartId && i.ItemId == cartItemId);
            if (cartItem == null)//item not exist => put item inside ShoppingCart
                return Json("NOT EXISTS!");

            //If we reached here: the item inside the cart => and we need to update the Quantity
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


        //Empty The Cart, DA!
        public IActionResult EmptyCart()
        {
            var cart = GetShoppingCart();
            //returns a list of all CartItems that Associated to Customer ShoppingCart
            var cartItems = _context.CartItem.Where(i => i.Cart.CartId == cart.CartId);
            foreach (var item in cartItems)
                _context.CartItem.Remove(item);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////









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

