using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using static NuGet.Packaging.PackagingConstants;

namespace METALLIST.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Внедрение ApplicationDbContext через конструктор
        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index(int? page, string searchString, bool isPartial = false)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Получаем список клиентов из базы данных
            var customers = _db.Customers
                .Where(o => string.IsNullOrEmpty(searchString) || o.FullName.Contains(searchString))
                .OrderBy(o => o.FullName)
                .ToPagedList(pageNumber, pageSize);

            if (isPartial)
            {
                ViewData["searchString"] = searchString;
                return PartialView("_CustomerListPartial", customers);
            }

            ViewData["searchString"] = searchString;

            return View(customers);
        }
        public IActionResult Search(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var customers = _db.Customers
                .Where(o => string.IsNullOrEmpty(searchString) || o.FullName.Contains(searchString))
                .OrderBy(o => o.FullName)
                .ToPagedList(pageNumber, pageSize);

            ViewData["searchString"] = searchString; // Передача строки поиска

            return PartialView("_CustomerListPartial", customers);
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            _db.Customers.Add(customer);
            _db.SaveChanges();

            int pageSize = 10;
            int pageNumber = 1;

            // Возвращаем обновленный список клиентов
            var customers = _db.Customers
                .OrderBy(o => o.FullName)
                .ToPagedList(pageNumber, pageSize);

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

            var customersOrders = _db.Orders.Where(c => c.CustomerId == id).ToList();
            _db.Orders.RemoveRange(customersOrders);

            int pageSize = 10;
            int pageNumber = 1;
            // Удаляем клиента
            _db.Customers.Remove(customer);
            _db.SaveChanges();

            // Возвращаем обновленный список клиентов
            var customers = _db.Customers
                .OrderBy(o => o.FullName)
                .ToPagedList(pageNumber, pageSize);
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
