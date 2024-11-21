using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace METALLIST.Controllers
{
    public class MaterialController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
