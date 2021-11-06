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
            var bekamiContext = _context.Product.Include(p => p.Color);
            return View(await bekamiContext.ToListAsync());
        }
        //showing only women t-shirts
        [Route("/women")]
        public async Task<IActionResult> Women()
        {
            var bekamiContext = _context.Product.Where(p => p.Gender == 0).Where(p => p.IsAvailable);
            return View("Shop", await bekamiContext.ToListAsync());
        }
        //showing only men t-shirts 
        [Route("/men")]
        public async Task<IActionResult> Men()
        {
            var bekamiContext = _context.Product.Where(p => p.Gender != 0).Where(p => p.IsAvailable);
            return View("Shop",await bekamiContext.ToListAsync());
        }


        [Route("/products/search")]
        public async Task<IActionResult> Shop(String searchString)
        {
            var bekamiContext = _context.Product.Where(p => p.IsAvailable).Where(p=>p.Name.Contains(searchString));
            return View("Shop", await bekamiContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Color)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Size,Gender,Price,IsAvailable,Imagepath,Imagepath2,Description,ColorId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ColorId"] = new SelectList(_context.Color, "ColorId", "Name", product.ColorId);
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
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Size,Gender,Price,IsAvailable,Imagepath,Imagepath2,Description,ColorId")] Product product)
        {
            if (id != product.Id)
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
                    if (!ProductExists(product.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
            return _context.Product.Any(e => e.Id == id);
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            return View(await _context.Product.Where(p => p.Name != "Not Found").Include(i => i.Color).ToListAsync());
        }
    }
}
