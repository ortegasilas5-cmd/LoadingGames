using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
	public class EsqueciSenhaController : Controller
	{
		private readonly ApplicationDbContext _context;

		public EsqueciSenhaController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				ViewBag.Erro = "Digite seu e-mail.";
				return View();
			}

			var usuario = await _context.Usuarios
				.FirstOrDefaultAsync(u => u.Email == email);

			if (usuario == null)
			{
				ViewBag.Erro = "E-mail não encontrado.";
				return View();
			}

			TempData["EmailRecuperacao"] = email;

			return RedirectToAction("NovaSenha");
		}

		[HttpGet]
		public IActionResult NovaSenha()
		{
			if (TempData["EmailRecuperacao"] == null)
			{
				return RedirectToAction("Index");
			}

			TempData.Keep("EmailRecuperacao");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> NovaSenha(
			string novaSenha,
			string confirmarSenha)
		{
			var email = TempData["EmailRecuperacao"]?.ToString();

			if (string.IsNullOrWhiteSpace(email))
			{
				return RedirectToAction("Index");
			}

			if (string.IsNullOrWhiteSpace(novaSenha) ||
				string.IsNullOrWhiteSpace(confirmarSenha))
			{
				ViewBag.Erro = "Preencha todos os campos.";
				TempData.Keep("EmailRecuperacao");
				return View();
			}

			if (novaSenha != confirmarSenha)
			{
				ViewBag.Erro = "As senhas não coincidem.";
				TempData.Keep("EmailRecuperacao");
				return View();
			}

			var usuario = await _context.Usuarios
				.FirstOrDefaultAsync(u => u.Email == email);

			if (usuario == null)
			{
				ViewBag.Erro = "Usuário não encontrado.";
				return View();
			}

			var passwordHasher = new PasswordHasher<Usuario>();

			usuario.SenhaHash = passwordHasher.HashPassword(
				usuario,
				novaSenha
			);

			await _context.SaveChangesAsync();

			TempData.Remove("EmailRecuperacao");
			TempData["Sucesso"] = "Senha alterada com sucesso!";

			return RedirectToAction("Index", "Login");
		}
	}
}