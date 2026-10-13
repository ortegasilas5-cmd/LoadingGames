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
            return HttpContext.Session.GetInt32("UsuarioId") != null &&
                   HttpContext.Session.GetString("UsuarioTipo") == "Administrador";
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!EhAdministrador()) return StatusCode(403);

            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync();

            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarStatus(int id)
        {
            if (!EhAdministrador()) return StatusCode(403);

            int adminId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            if (id == adminId)
            {
                TempData["Erro"] = "Voce nao pode bloquear sua propria conta.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Mantem ao menos um administrador ativo.
            if (usuario.TipoUsuario == "Administrador" &&
                usuario.StatusConta == "Ativo")
            {
                int adminsAtivos = await _context.Usuarios.CountAsync(u =>
                    u.TipoUsuario == "Administrador" && u.StatusConta == "Ativo");
                if (adminsAtivos <= 1)
                {
                    TempData["Erro"] = "E necessario manter um administrador ativo.";
                    return RedirectToAction(nameof(Index));
                }
            }

            usuario.StatusConta = usuario.StatusConta == "Ativo" ? "Inativo" : "Ativo";
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = $"Conta de {usuario.NomeUsuario}: {usuario.StatusConta}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarTipo(int id)
        {
            if (!EhAdministrador()) return StatusCode(403);

            int adminId = HttpContext.Session.GetInt32("UsuarioId")!.Value;
            if (id == adminId)
            {
                TempData["Erro"] = "Voce nao pode alterar sua propria permissao.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (usuario.TipoUsuario == "Administrador")
            {
                int adminsAtivos = await _context.Usuarios.CountAsync(u =>
                    u.TipoUsuario == "Administrador" && u.StatusConta == "Ativo");
                if (usuario.StatusConta == "Ativo" && adminsAtivos <= 1)
                {
                    TempData["Erro"] = "E necessario manter um administrador ativo.";
                    return RedirectToAction(nameof(Index));
                }
                usuario.TipoUsuario = "Comum";
            }
            else
            {
                usuario.TipoUsuario = "Administrador";
            }

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = $"Permissao de {usuario.NomeUsuario}: {usuario.TipoUsuario}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
