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

        // CATÁLOGO
        public IActionResult Index(int? categoria)
        {
            var produtos = _context.Produtos.ToList();

            var categorias = _context.Categorias.ToList();

            ViewBag.Categorias = categorias;

            if (categoria.HasValue)
            {
                produtos = produtos
                    .Where(p => p.IdCategoria == categoria.Value)
                    .ToList();
            }

            return View(produtos);
        }

        // DETALHES DO JOGO
        public IActionResult Details(int id)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // ==========================
        // CRUD - CADASTRAR
        // ==========================

        // Abre o formulário
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Fabricantes = _context.Fabricantes.ToList();

            return View();
        }

        // Salva o jogo no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Produtos.Add(produto);
                _context.SaveChanges();

                return RedirectToAction(nameof(Crud));
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Fabricantes = _context.Fabricantes.ToList();

            return View(produto);
        }

        // ==========================
        // CRUD - EDITAR
        // ==========================

        // Abre o formulário de edição
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Fabricantes = _context.Fabricantes.ToList();

            return View(produto);
        }

        // Salva as alterações
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Produtos.Update(produto);
                _context.SaveChanges();

                return RedirectToAction(nameof(Crud));
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Fabricantes = _context.Fabricantes.ToList();

            return View(produto);
        }

        // ==========================
        // CRUD - EXCLUIR
        // ==========================

        // Mostra a confirmação
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // Exclui definitivamente
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var produto = _context.Produtos
                .FirstOrDefault(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return RedirectToAction(nameof(Crud));
        }

        // ==========================
        // LISTA ADMINISTRATIVA
        // ==========================

        public IActionResult Crud()
        {
            var produtos = _context.Produtos.ToList();

            return View(produtos);
        }
    }
}

