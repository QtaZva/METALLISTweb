using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Index()
        {
            // Получаем список заказов с клиентами
            var orders = _db.Orders
                .Include(o => o.Customer) // Подгружаем связанных клиентов
                .ToList();

            // Передаем список клиентов в ViewBag для выпадающего списка, если он используется
            ViewBag.Customers = _db.Customers.ToList();
            ViewBag.Materials = _db.Materials.ToList();

            return View(orders);
        }

        // Добавление нового заказа
        [HttpPost]
        public IActionResult CreateOrder(Order order/*, List<OrderMaterial> orderMaterials*/)
        {
            try
            {
                // Добавляем заказ
                _db.Orders.Add(order);
                _db.SaveChanges();

                // Добавляем материалы к заказу
                /*if (orderMaterials != null && orderMaterials.Any())
                {
                    foreach (var orderMaterial in orderMaterials)
                    {
                        orderMaterial.OrderId = order.Id;
                        _db.OrderMaterials.Add(orderMaterial);
                    }
                    _db.SaveChanges();
                }*/

                // Возвращаем обновленный список заказов
                var orders = _db.Orders
                    .Include(o => o.Customer)
                    .ToList();

                return PartialView("_OrderListPartial", orders);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка при добавлении заказа: {ex.Message}");
                return StatusCode(500, "Произошла ошибка при добавлении заказа.");
            }
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

            // Возвращаем обновленный список заказов
            var orders = _db.Orders
                .Include(o => o.Customer)
                .ToList();
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
            _db.OrderMaterials.RemoveRange(order.OrderMaterials); // Удаляем старые связи
            if (orderMaterials != null && orderMaterials.Any())
            {
                foreach (var material in orderMaterials)
                {
                    material.OrderId = order.Id;
                    _db.OrderMaterials.Add(material);
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
