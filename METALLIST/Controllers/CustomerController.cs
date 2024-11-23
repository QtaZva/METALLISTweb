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
        public IActionResult EditCustomer(int id)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                return NotFound("Клиент не найден.");
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult EditCustomer(Customer updatedCustomer)
        {
            var customer = _db.Customers.FirstOrDefault(c => c.Id == updatedCustomer.Id);
            if (customer == null)
            {
                return NotFound("Клиент не найден.");
            }

            // Обновляем данные клиента
            customer.FullName = updatedCustomer.FullName;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.Requisites = updatedCustomer.Requisites;
            customer.OrganizationName = updatedCustomer.OrganizationName;

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
