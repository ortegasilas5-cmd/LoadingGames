using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Exibe o carrinho do usuário logado
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // Se não estiver logado, manda para o login
            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(c => c.IdUsuario == usuarioId.Value);

            return View(carrinho);
        }

        // Adiciona um jogo ao carrinho
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(int idJogo)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Confere se o jogo existe e está ativo
            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(j =>
                    j.IdJogo == idJogo &&
                    j.Ativo);

            if (jogo == null)
            {
                return NotFound();
            }

            // Procura o carrinho do usuário
            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c =>
                    c.IdUsuario == usuarioId.Value);

            // Se ainda não existir, cria
            if (carrinho == null)
            {
                carrinho = new Carrinho
                {
                    IdUsuario = usuarioId.Value,
                    DataCriacao = DateTime.Now
                };

                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            // Verifica se o jogo já está no carrinho
            bool jogoJaAdicionado = await _context.ItensCarrinho
                .AnyAsync(i =>
                    i.IdCarrinho == carrinho.IdCarrinho &&
                    i.IdJogo == idJogo);

            if (jogoJaAdicionado)
            {
                TempData["Aviso"] =
                    $"{jogo.Nome} já está no seu carrinho.";

                return RedirectToAction(nameof(Index));
            }

            var item = new ItemCarrinho
            {
                IdCarrinho = carrinho.IdCarrinho,
                IdJogo = idJogo,
                DataAdicao = DateTime.Now
            };

            _context.ItensCarrinho.Add(item);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                $"{jogo.Nome} foi adicionado ao carrinho.";

            return RedirectToAction(nameof(Index));
        }

        // Remove um jogo do carrinho
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int idItemCarrinho)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Procura o item e garante que ele pertence
            // ao carrinho do usuário logado
            var item = await _context.ItensCarrinho
                .Include(i => i.Carrinho)
                .Include(i => i.Jogo)
                .FirstOrDefaultAsync(i =>
                    i.IdItemCarrinho == idItemCarrinho &&
                    i.Carrinho.IdUsuario == usuarioId.Value);

            if (item == null)
            {
                return NotFound();
            }

            string nomeJogo = item.Jogo.Nome;

            _context.ItensCarrinho.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                $"{nomeJogo} foi removido do carrinho.";

            return RedirectToAction(nameof(Index));
        }
    }
}