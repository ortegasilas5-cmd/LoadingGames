using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoadingGames.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly LoadingGamesContext _context;

        public ProdutoController(LoadingGamesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var produtos = _context.Produtos.ToList();

            return View(produtos);
        }

        public IActionResult Details(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }
    }
}