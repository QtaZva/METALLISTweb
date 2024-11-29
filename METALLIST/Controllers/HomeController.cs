using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace METALLIST.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Take(3)
                .ToListAsync();

            var materials = await _context.Materials
                .Take(3)
                .ToListAsync();

            var orders = await _context.Orders
                .Take(3)
                .ToListAsync();

            var model = new HomeViewModel
            {
                Customers = customers,
                Materials = materials,
                Orders = orders
            };

            return View(model);
        }
    }

    public class HomeViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Material> Materials { get; set; }
        public List<Order> Orders { get; set; }
    }
}

