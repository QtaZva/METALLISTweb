using METALLIST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using X.PagedList.Extensions;
using static NuGet.Packaging.PackagingConstants;

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
        public IActionResult Index(int? page, string searchString, bool isPartial = false)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var groupedOrderMaterials = _db.OrderMaterials
               .Include(om => om.Order)
               .Include(om => om.Material)
               // Фильтрация записей по строке поиска
               .Where(om => string.IsNullOrEmpty(searchString) ||
                            (om.Order != null && om.Order.Name.Contains(searchString)))
               .AsEnumerable() // Переход к обработке в памяти
               .GroupBy(om => om.Order)
               .Select(group => new
               {
                   OrderId = group.Key?.Id,
                   OrderName = group.Key?.Name,
                   OrderDescription = group.Key?.Description,
                   MaterialsSummary = string.Join("<br />", group.Select(om => $"{om.Material?.Name} - {om.Quantity} шт."))
               })
               .ToPagedList(pageNumber, pageSize); // Пагинация

            if (isPartial)
            {
                ViewData["searchString"] = searchString;
                return PartialView("_OrderMaterialListPartial", groupedOrderMaterials);
            }
            ViewData["searchString"] = searchString;
            return View(groupedOrderMaterials);
        }

        [HttpGet]
        public IActionResult Search(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var groupedOrderMaterials = _db.OrderMaterials
                .Include(om => om.Order)
                .Include(om => om.Material)
                // Фильтрация по строке поиска
                .Where(om => string.IsNullOrEmpty(searchString) ||
                             (om.Order != null && om.Order.Name.Contains(searchString)))
                .AsEnumerable() // Для дальнейшей обработки в памяти
                .GroupBy(om => om.Order)
                .Select(group => new
                {
                    OrderId = group.Key?.Id,
                    OrderName = group.Key?.Name,
                    OrderDescription = group.Key?.Description,
                    MaterialsSummary = string.Join("<br />", group.Select(om => $"{om.Material?.Name} - {om.Quantity} шт."))
                })
                .ToPagedList(pageNumber, pageSize); // Пагинация

            ViewData["searchString"] = searchString; // Передача строки поиска

            return PartialView("_OrderMaterialListPartial", groupedOrderMaterials);
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
        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            var OrderMaterials = _db.OrderMaterials
                .Include(om => om.Order)
                .Include(om => om.Material)
                // Фильтрация по строке поиска
                .Where(om => om.Order.Id == id)
                .ToList();

            return View(OrderMaterials);
        }
        public IActionResult DeleteMaterialFromOrder(int orderId, int materialId)
        {
            var orderMaterial = _db.OrderMaterials.Where(om => om.MaterialId == materialId && om.OrderId == orderId).FirstOrDefault();
            if (orderMaterial == null)
                return NotFound();
            _db.OrderMaterials.Remove(orderMaterial);
            _db.SaveChanges();
            return RedirectToAction("OrderDetails", new {id = orderId});
        }
    }
}
