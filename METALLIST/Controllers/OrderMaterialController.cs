using METALLIST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace METALLIST.Controllers
{
    public class OrderMaterialController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderMaterialController(ApplicationDbContext db) 
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // Загружаем данные с включением связанных сущностей (Orders и Materials)
            var orderMaterials = _db.OrderMaterials
                .Include(om => om.Order)
                .Include(om => om.Material)
                .ToList();

            return View(orderMaterials);
        }
        [HttpGet]
        public IActionResult AddMaterial(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.Materials = _db.Materials.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult AddMaterial(int orderId, int materialId, int quantity) 
        {
            // Проверка существования заказа
            var order = _db.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден.");
            }

            // Проверка существования материала
            var material = _db.Materials.Find(materialId);
            if (material == null)
            {
                return NotFound("Материал не найден.");
            }

            // Проверка на дублирование записи
            var existingEntry = _db.OrderMaterials
                .FirstOrDefault(om => om.OrderId == orderId && om.MaterialId == materialId);

            if (existingEntry != null)
            {
                // Обновление количества, если материал уже добавлен
                existingEntry.Quantity += quantity;
                _db.OrderMaterials.Update(existingEntry);
            }
            else
            {
                // Добавление новой записи
                var orderMaterial = new OrderMaterial
                {
                    OrderId = orderId,
                    MaterialId = materialId,
                    Quantity = quantity
                };
                _db.OrderMaterials.Add(orderMaterial);
            }

            // Сохранение изменений
            _db.SaveChanges();

            return RedirectToAction("Index", "Order");
        }
    }
}
