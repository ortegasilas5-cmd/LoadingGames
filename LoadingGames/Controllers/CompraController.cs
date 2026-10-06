using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class CompraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompraController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(c =>
                    c.IdUsuario == usuarioId.Value);

            if (carrinho == null || !carrinho.Itens.Any())
            {
                TempData["Aviso"] = "Seu carrinho está vazio.";

                return RedirectToAction("Index", "Carrinho");
            }

            return View(carrinho);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(string formaPagamento)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            string[] formasPermitidas =
            {
                "Cartao",
                "Pix",
                "Boleto"
            };

            if (string.IsNullOrWhiteSpace(formaPagamento) ||
                !formasPermitidas.Contains(formaPagamento))
            {
                TempData["Aviso"] = "Selecione uma forma de pagamento válida.";

                return RedirectToAction(nameof(Index));
            }

            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(c =>
                    c.IdUsuario == usuarioId.Value);

            if (carrinho == null || !carrinho.Itens.Any())
            {
                TempData["Aviso"] = "Seu carrinho está vazio.";

                return RedirectToAction("Index", "Carrinho");
            }

            var idsJogosCarrinho = carrinho.Itens
                .Select(i => i.IdJogo)
                .ToList();

            bool possuiJogoDoCarrinho = await _context.PossesJogos
                .AnyAsync(p =>
                    p.IdUsuario == usuarioId.Value &&
                    idsJogosCarrinho.Contains(p.IdJogo));

            if (possuiJogoDoCarrinho)
            {
                TempData["Aviso"] =
                    "Um ou mais jogos do carrinho já pertencem à sua biblioteca.";

                return RedirectToAction("Index", "Carrinho");
            }

            decimal valorTotal = carrinho.Itens
                .Sum(i => i.Jogo.Preco);

            await using var transacao =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var compra = new Compra
                {
                    IdUsuario = usuarioId.Value,
                    DataCompra = DateTime.Now,
                    ValorTotal = valorTotal,
                    FormaPagamento = formaPagamento,
                    StatusCompra = "Concluida"
                };

                _context.Compras.Add(compra);

                // Precisamos do IdCompra gerado pelo MySQL
                // antes de criar os itens e as posses.
                await _context.SaveChangesAsync();

                foreach (var itemCarrinho in carrinho.Itens)
                {
                    var itemCompra = new ItemCompra
                    {
                        IdCompra = compra.IdCompra,
                        IdJogo = itemCarrinho.IdJogo,
                        PrecoUnitario = itemCarrinho.Jogo.Preco
                    };

                    _context.ItensCompra.Add(itemCompra);

                    var posse = new PosseJogo
                    {
                        IdUsuario = usuarioId.Value,
                        IdJogo = itemCarrinho.IdJogo,
                        IdCompra = compra.IdCompra,
                        DataAquisicao = DateTime.Now
                    };

                    _context.PossesJogos.Add(posse);
                }

                _context.ItensCarrinho.RemoveRange(carrinho.Itens);

                await _context.SaveChangesAsync();

                await transacao.CommitAsync();

                TempData["Sucesso"] =
                    "Compra realizada com sucesso!";

                return RedirectToAction(nameof(Sucesso), new
                {
                    id = compra.IdCompra
                });
            }
            catch
            {
                await transacao.RollbackAsync();

                TempData["Aviso"] =
                    "Não foi possível finalizar a compra. Tente novamente.";

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sucesso(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var compra = await _context.Compras
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Jogo)
                .FirstOrDefaultAsync(c =>
                    c.IdCompra == id &&
                    c.IdUsuario == usuarioId.Value);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }
    }
}