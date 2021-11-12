using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using bekami.Data;
using bekami.Models;
using Microsoft.AspNetCore.Authorization;


namespace bekami.Controllers
{
    public class ProductsController : Controller
    {
        private readonly bekamiContext _context;

        public ProductsController(bekamiContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var bekamiContext = _context.Product.Include(p => p.Color).Include(p => p.Category);
            return View(await bekamiContext.ToListAsync());
        }

        //showing Products on shop page 
        [Route("/shop")]
        public async Task<IActionResult> Shop()
        {
            var bekamiContext = _context.Product.Where(p => p.IsAvailable);
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            return View(await bekamiContext.ToListAsync());
        }


        //[Route("/products/search")]
        //public async Task<IActionResult> Shop(String searchString)
        //{
        //    var bekamiContext = _context.Product.Where(p => p.IsAvailable).Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
        //    ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name");
        //    ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
        //    return View("Shop", await bekamiContext.ToListAsync());
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Color)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name");
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Size,Gender,Price,IsAvailable,Imagepath,Imagepath2,Description,ColorId,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name", product.ColorId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name", product.ColorId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name",product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Size,Gender,Price,IsAvailable,Imagepath,Imagepath2,Description,ColorId,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name", product.ColorId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Color)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            return View(await _context.Product.Where(p => p.Name != "Not Found").Include(i => i.Color).ToListAsync());
        }


        public async Task<IActionResult> ProductPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //public async Task<IActionResult> GetProducts (string sortby, int sgender, Color scolor,Category scategory)
        //{
        //    var filterproducts = await _context.Product.Include(m => m.Category).Include(m => m.Color).Where(m => m.IsAvailable).ToListAsync();
        //    if(sortby.Equals("alphabetical"))
        //    {
        //        filterproducts.Sort((a, b) => a.Name.CompareTo(b.Name));    
        //    }
        //    if (sortby.Equals("low2high"))
        //    {
        //        filterproducts.Sort((a, b) => a.Price.CompareTo(b.Price));
        //    }
        //    if (sortby.Equals("high2low"))
        //    {
        //        filterproducts.Sort((a, b) => a.Price.CompareTo(b.Price));
        //        filterproducts.Reverse();
        //    }
        //    if (sortby.Equals("newest"))
        //    {
        //        filterproducts.Sort((a, b) => a.ProductId.CompareTo(b.Price));
        //    }


        //    return Json(filterproducts);
        //}

    }
}
