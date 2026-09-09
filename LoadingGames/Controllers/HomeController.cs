using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LoadingGames.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Mensagem"] = "Olá! Este é o meu primeiro site em ASP.NET Core MVC.";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
