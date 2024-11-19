using METALLIST.Models;
using Microsoft.AspNetCore.Mvc;

namespace METALLIST.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Внедрение ApplicationDbContext через конструктор
        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            // Сохранение клиента в базу данных
            _db.Customers.Add(customer);
            _db.SaveChanges();

            return RedirectToAction("Index", "Home"); // Перенаправление на список клиентов
            
        }
    }
}
