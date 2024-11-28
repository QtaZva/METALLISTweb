using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace METALLIST.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        // Внедрение ApplicationDbContext через конструктор
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Отображение списка заказов
        [HttpGet]
        public IActionResult Index(int? page, string searchString, bool isPartial = false)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var orders = _db.Orders
                .Include(o => o.Customer)
                .Where(o => string.IsNullOrEmpty(searchString) || o.Name.Contains(searchString))
                .OrderBy(o => o.Name)
                .ToPagedList(pageNumber, pageSize);

            if (isPartial)
            {
                ViewData["searchString"] = searchString;
                return PartialView("_OrderListPartial", orders);
            }

            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Materials = _db.Materials.ToList();
            ViewData["searchString"] = searchString;

            return View(orders);
        }

        public IActionResult Search(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var orders = _db.Orders
                .Include(o => o.Customer)
                .Where(o => string.IsNullOrEmpty(searchString) || o.Name.Contains(searchString))
                .OrderBy(o => o.Name)
                .ToPagedList(pageNumber, pageSize);

            ViewData["searchString"] = searchString; // Передача строки поиска

            return PartialView("_OrderListPartial", orders);
        }

        // Добавление нового заказа
        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            // Добавляем заказ
            _db.Orders.Add(order);
            _db.SaveChanges();

            // Получаем обновленный список заказов с учётом пагинации
            int pageSize = 10;
            int pageNumber = 1; // Возвращаем первую страницу после добавления

            var orders = _db.Orders
                .Include(o => o.Customer)
                .OrderBy(o => o.Name)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_OrderListPartial", orders);
        }

        // Удаление заказа
        [HttpPost]
        public IActionResult DeleteOrder(int id)
        {
            // Ищем заказ по ID
            var order = _db.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Заказ не найден.");
            }

            // Удаляем связанные материалы
            var orderMaterials = _db.OrderMaterials.Where(om => om.OrderId == id).ToList();
            _db.OrderMaterials.RemoveRange(orderMaterials);

            // Удаляем заказ
            _db.Orders.Remove(order);
            _db.SaveChanges();

            int pageSize = 10;
            int pageNumber = 1; // Первая страница после удаления

            var orders = _db.Orders
                .Include(o => o.Customer)
                .OrderBy(o => o.Name)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_OrderListPartial", orders);
        }

        // Редактирование заказа: загрузка данных
        [HttpGet]
        public IActionResult EditOrder(int id)
        {
            var order = _db.Orders
                .Include(o => o.OrderMaterials) // Подгружаем связанные материалы
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound("Заказ не найден.");
            }

            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Materials = _db.Materials.ToList();

            return View(order);
        }

        // Редактирование заказа: сохранение изменений
        [HttpPost]
        public IActionResult EditOrder(Order updatedOrder, List<OrderMaterial> orderMaterials)
        {
            var order = _db.Orders.Include(o => o.OrderMaterials).FirstOrDefault(o => o.Id == updatedOrder.Id);
            if (order == null)
            {
                return NotFound("Заказ не найден.");
            }

            // Обновляем данные заказа
            order.Name = updatedOrder.Name;
            order.Description = updatedOrder.Description;
            order.Price = updatedOrder.Price;
            order.TypeOfPaint = updatedOrder.TypeOfPaint;
            order.Difficulty = updatedOrder.Difficulty;
            order.CustomerId = updatedOrder.CustomerId;

            // Обновляем связанные материалы
            /*_db.OrderMaterials.RemoveRange(order.OrderMaterials); // Удаляем старые связи
            if (orderMaterials != null && orderMaterials.Any())
            {
                foreach (var material in orderMaterials)
                {
                    material.OrderId = order.Id;
                    _db.OrderMaterials.Add(material);
                }
            }*/

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
