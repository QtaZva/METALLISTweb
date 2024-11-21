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

        [HttpGet]
        public IActionResult Index()
        {
            // Получаем список клиентов из базы данных
            var customers = _db.Customers.ToList();
            return View(customers);
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();

            // Возвращаем обновленный список клиентов
            var customers = _db.Customers.ToList();
            return PartialView("_CustomerListPartial", customers);

        }

        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            // Ищем клиента по ID
            var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound("Клиент не найден.");
            }

            // Удаляем клиента
            _db.Customers.Remove(customer);
            _db.SaveChanges();

            // Возвращаем обновленный список клиентов
            var customers = _db.Customers.ToList();
            return PartialView("_CustomerListPartial", customers);
        }

        [HttpGet]
        public IActionResult GetCustomer(int id)
        {
            var customer = _db.Customers.Where(c => c.Id == id).FirstOrDefault();
            if (customer == null) return NotFound("Клиент не найден.");
            return Json(customer);
        }

        [HttpPost]
        public IActionResult UpdateCustomer(Customer customer)
        {
            if (customer.Id == 0)
            {
                return BadRequest("ID клиента отсутствует.");
            }

            var existingCustomer = _db.Customers.Where(c => c.Id == customer.Id).FirstOrDefault();
            if (existingCustomer == null)
            {
                return NotFound("Клиент не найден.");
            }

            existingCustomer.FullName = customer.FullName;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Requisites = customer.Requisites;
            existingCustomer.OrganizationName = customer.OrganizationName;

            _db.SaveChanges();

            var customers = _db.Customers.ToList();
            return PartialView("_CustomerListPartial", customers);
        }


    }
}
