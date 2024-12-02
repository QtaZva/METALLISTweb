using METALLIST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

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
        public ActionResult Index(int? page, string searchString, bool isPartial = false)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var materials = _db.Materials
                .ToList()
                .Where(m => string.IsNullOrEmpty(searchString) || m.Name.Contains(searchString))
                .OrderBy(m => m.Name)
                .ToPagedList(pageNumber, pageSize);

            if (isPartial)
            {
                ViewData["searchString"] = searchString;
                return PartialView("_MaterialListPartial", materials);
            }

            ViewData["searchString"] = searchString;

            return View(materials);
        }
        public IActionResult Search(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var materials = _db.Materials
                .Where(m => string.IsNullOrEmpty(searchString) || m.Name.Contains(searchString))
                .OrderBy(m => m.Name)
                .ToPagedList(pageNumber, pageSize);

            ViewData["searchString"] = searchString;

            return PartialView("_MaterialListPartial", materials);
        }

        [HttpPost]
        public IActionResult AddMaterial(Material material)
        {
            _db.Materials.Add(material);
            _db.SaveChanges();

            int pageSize = 10;
            int pageNumber = 1; 

            var materials = _db.Materials
                .OrderBy(m => m.Name)
                .ToPagedList(pageNumber, pageSize);
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
            var materialsForOrder = _db.OrderMaterials.Where(m => m.MaterialId == id).ToList();
            _db.OrderMaterials.RemoveRange(materialsForOrder);

            _db.Materials.Remove(material);
            _db.SaveChanges();

            int pageSize = 10;
            int pageNumber = 1; 

            var materials = _db.Materials
                .OrderBy(o => o.Name)
                .ToPagedList(pageNumber, pageSize);

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
