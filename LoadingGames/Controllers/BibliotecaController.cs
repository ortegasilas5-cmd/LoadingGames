using LoadingGames.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class BibliotecaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BibliotecaController(ApplicationDbContext context)
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

            var jogos = await _context.PossesJogos
                .Where(p => p.IdUsuario == usuarioId.Value)
                .Include(p => p.Jogo)
                .OrderByDescending(p => p.DataAquisicao)
                .ToListAsync();

            return View(jogos);
        }
    }
}