using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class JogoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogoController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // CATÁLOGO
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(string? pesquisa, int? genero)
        {
            var jogos = _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .Include(j => j.Generos)
                .Where(j => j.Ativo)
                .AsQueryable();

            // Pesquisa pelo nome
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim();

                var termos = pesquisa
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                jogos = jogos.Where(j =>
                    j.Nome.Contains(pesquisa) ||
                    termos.All(termo =>
                        j.Nome.Contains(termo)));
            }

            // Filtro por gênero
            if (genero.HasValue)
            {
                jogos = jogos.Where(j =>
                    j.Generos.Any(g =>
                        g.IdGenero == genero.Value));
            }

            ViewBag.Generos = await _context.Generos
                .OrderBy(g => g.Nome)
                .ToListAsync();

            ViewBag.Pesquisa = pesquisa;
            ViewBag.GeneroSelecionado = genero;

            return View(await jogos
                .OrderBy(j => j.Nome)
                .ToListAsync());
        }


        // =========================================================
        // DETALHES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var jogo = await _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .Include(j => j.Generos)
                .FirstOrDefaultAsync(j =>
                    j.IdJogo == id && j.Ativo);

            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }


        // =========================================================
        // CRUD - LISTAGEM ADMINISTRATIVA
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Crud()
        {
            var jogos = await _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .OrderBy(j => j.Nome)
                .ToListAsync();

            return View(jogos);
        }


        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarListas();

            return View();
        }


        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jogo jogo)
        {
            // As propriedades de navegação não vêm do formulário.
            ModelState.Remove(nameof(Jogo.Desenvolvedor));
            ModelState.Remove(nameof(Jogo.Publicadora));
            ModelState.Remove(nameof(Jogo.Generos));

            if (!ModelState.IsValid)
            {
                await CarregarListas();

                return View(jogo);
            }

            _context.Jogos.Add(jogo);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Crud));
        }


        // =========================================================
        // EDIT - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(j => j.IdJogo == id);

            if (jogo == null)
            {
                return NotFound();
            }

            await CarregarListas();

            return View(jogo);
        }


        // =========================================================
        // EDIT - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Jogo jogo)
        {
            if (id != jogo.IdJogo)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(Jogo.Desenvolvedor));
            ModelState.Remove(nameof(Jogo.Publicadora));
            ModelState.Remove(nameof(Jogo.Generos));

            if (!ModelState.IsValid)
            {
                await CarregarListas();

                return View(jogo);
            }

            var jogoBanco = await _context.Jogos
                .FirstOrDefaultAsync(j => j.IdJogo == id);

            if (jogoBanco == null)
            {
                return NotFound();
            }

            jogoBanco.Nome = jogo.Nome;
            jogoBanco.Descricao = jogo.Descricao;
            jogoBanco.Preco = jogo.Preco;
            jogoBanco.ImagemPrincipalUrl = jogo.ImagemPrincipalUrl;
            jogoBanco.DataLancamento = jogo.DataLancamento;
            jogoBanco.ClassificacaoIndicativa =
                jogo.ClassificacaoIndicativa;
            jogoBanco.IdDesenvolvedor = jogo.IdDesenvolvedor;
            jogoBanco.IdPublicadora = jogo.IdPublicadora;
            jogoBanco.Ativo = jogo.Ativo;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Crud));
        }


        // =========================================================
        // DELETE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(j => j.IdJogo == id);

            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }


        // =========================================================
        // DELETE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Jogo jogo)
        {
            var jogoBanco = await _context.Jogos
                .FirstOrDefaultAsync(j =>
                    j.IdJogo == jogo.IdJogo);

            if (jogoBanco == null)
            {
                return NotFound();
            }

            _context.Jogos.Remove(jogoBanco);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Crud));
        }


        // =========================================================
        // MÉTODO AUXILIAR
        // =========================================================

        private async Task CarregarListas()
        {
            ViewBag.Desenvolvedores =
                await _context.Desenvolvedores
                    .OrderBy(d => d.Nome)
                    .ToListAsync();

            ViewBag.Publicadoras =
                await _context.Publicadoras
                    .OrderBy(p => p.Nome)
                    .ToListAsync();
        }
    }
}