using bekami.Data;
using bekami.Models;
using Bekami.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bekami.Controllers
{
    public class HomeController : Controller
    {
        private readonly bekamiContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, bekamiContext _context)
        {
            _logger = logger;
            _dbContext = _context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Aboutus()
        {
            return View();
        }

        public async Task<IActionResult> Stores()
        {
            List<Branch> branches = await _dbContext.Branch.ToListAsync();
            return View(branches);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
