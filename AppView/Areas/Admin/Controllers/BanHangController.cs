using Microsoft.AspNetCore.Mvc;

namespace AppView.Areas.Admin.Controllers
{
    public class BanHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
