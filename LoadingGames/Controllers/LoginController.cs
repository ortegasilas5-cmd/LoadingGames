using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
	public class LoginController : Controller
	{
		private readonly ApplicationDbContext _context;

		public LoginController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Abre a tela de login
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		// Processa o login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(string email, string senha)
		{
			if (string.IsNullOrWhiteSpace(email) ||
				string.IsNullOrWhiteSpace(senha))
			{
				ViewBag.Erro = "Informe o e-mail e a senha.";
				return View();
			}

			var usuario = await _context.Usuarios
				.FirstOrDefaultAsync(u => u.Email == email);

			if (usuario == null)
			{
				ViewBag.Erro = "E-mail ou senha incorretos.";
				return View();
			}

			var passwordHasher = new PasswordHasher<Usuario>();

			var resultado = passwordHasher.VerifyHashedPassword(
				usuario,
				usuario.SenhaHash,
				senha
			);

			if (resultado == PasswordVerificationResult.Success ||
				resultado == PasswordVerificationResult.SuccessRehashNeeded)
			{
				// Salva o nome e o e-mail do usuário na sessão
				HttpContext.Session.SetString(
					"UsuarioNome",
					usuario.Nome
				);

				HttpContext.Session.SetString(
					"UsuarioEmail",
					usuario.Email
				);

				return RedirectToAction("Index", "Home");
			}

			ViewBag.Erro = "E-mail ou senha incorretos.";
			return View();
		}
	}
}