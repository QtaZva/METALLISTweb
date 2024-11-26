using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace METALLIST.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        private ApplicationDbContext _db;
        public MaterialController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var materials = _db.Materials.ToList();
            return View(materials);
        }

        [HttpPost]
        public IActionResult AddMaterial(Material material)
        {
            _db.Materials.Add(material);
            _db.SaveChanges();

            // Возвращаем обновленный список клиентов
            var materials = _db.Materials.ToList();
            return PartialView("_MaterialListPartial", materials);

        }

        [HttpPost]
        public IActionResult DeleteMaterial(int id)
        {
            var material = _db.Materials.FirstOrDefault(c => c.Id == id);
            if (material == null)
            {
                return NotFound("Материал не найден.");
            }

            _db.Materials.Remove(material);
            _db.SaveChanges();

            // Возвращаем обновленный список клиентов
            var materials = _db.Materials.ToList();
            return PartialView("_MaterialListPartial", materials);
        }
        [HttpGet]
        public IActionResult EditMaterial(int id)
        {
            var material = _db.Materials.FirstOrDefault(m => m.Id == id);
            if (material == null)
                return NotFound("Материал не найден");

            return View(material);
        }
        [HttpPost]
        public IActionResult EditMaterial(Material updatedMaterial)
        {
            var material = _db.Materials.FirstOrDefault(m => m.Id == updatedMaterial.Id);
            if (material == null)
                return NotFound("Материал не найден");

            material.Name = updatedMaterial.Name;
            material.Description = updatedMaterial.Description;
            material.Quantity = updatedMaterial.Quantity;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
