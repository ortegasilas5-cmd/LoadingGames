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

        [HttpGet]
        public async Task<IActionResult> Index(string? pesquisa, int? genero)
        {
            var jogos = _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .Include(j => j.Generos)
                .Where(j => j.Ativo)
                .AsQueryable();

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

            var avaliacoes = await _context.Avaliacoes
                .Include(a => a.Usuario)
                .Where(a => a.IdJogo == id)
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            ViewBag.Avaliacoes = avaliacoes;

            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            bool possuiJogo = false;
            Avaliacao? avaliacaoUsuario = null;

            if (usuarioId != null)
            {
                possuiJogo = await _context.PossesJogos
                    .AnyAsync(p =>
                        p.IdUsuario == usuarioId.Value &&
                        p.IdJogo == id);

                avaliacaoUsuario = await _context.Avaliacoes
                    .FirstOrDefaultAsync(a =>
                        a.IdUsuario == usuarioId.Value &&
                        a.IdJogo == id);
            }

            ViewBag.PossuiJogo = possuiJogo;
            ViewBag.AvaliacaoUsuario = avaliacaoUsuario;

            return View(jogo);
        }

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarListas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Jogo jogo)
        {
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
            jogoBanco.ClassificacaoIndicativa = jogo.ClassificacaoIndicativa;
            jogoBanco.IdDesenvolvedor = jogo.IdDesenvolvedor;
            jogoBanco.IdPublicadora = jogo.IdPublicadora;
            jogoBanco.Ativo = jogo.Ativo;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Crud));
        }

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