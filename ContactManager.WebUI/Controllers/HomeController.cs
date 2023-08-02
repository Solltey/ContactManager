using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ContactManager.WebUI.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}