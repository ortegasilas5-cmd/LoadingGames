using Microsoft.AspNetCore.Mvc;

namespace LoadingGames.Controllers
{
    public class ProdutoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewData["Id"] = id;

            return View();
        }
    }
}
