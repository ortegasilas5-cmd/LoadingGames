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

        // Catálogo
        [HttpGet]
        public async Task<IActionResult> Index(string? pesquisa, int? genero)
        {
            var jogos = _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .Include(j => j.Generos)
                .Where(j => j.Ativo)
                .AsQueryable();

            // Pesquisa pelo nome do jogo
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                jogos = jogos.Where(j =>
                    j.Nome.Contains(pesquisa));
            }

            // Filtro por gênero
            if (genero.HasValue)
            {
                jogos = jogos.Where(j =>
                    j.Generos.Any(g => g.IdGenero == genero.Value));
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

        // Detalhes de um jogo
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var jogo = await _context.Jogos
                .Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora)
                .Include(j => j.Generos)
                .FirstOrDefaultAsync(j => j.IdJogo == id && j.Ativo);

            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }
    }
}