
using LoadingGames.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class AdminUsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool EhAdministrador()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            string? tipoUsuario =
                HttpContext.Session.GetString("UsuarioTipo");

            return usuarioId != null &&
                   tipoUsuario == "Administrador";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!EhAdministrador())
            {
                return StatusCode(403);
            }

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
        }
    }
}
