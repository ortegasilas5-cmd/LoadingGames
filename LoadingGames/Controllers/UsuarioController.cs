using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Exibe a tela de cadastro
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        // Recebe os dados enviados pela tela de cadastro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(CadastroViewModel model)
        {
            // Verifica as validações definidas no CadastroViewModel
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verifica se o e-mail já está cadastrado
            bool emailExiste = await _context.Usuarios
                .AnyAsync(u => u.Email == model.Email);

            if (emailExiste)
            {
                ModelState.AddModelError(
                    "Email",
                    "Este e-mail já está cadastrado."
                );

                return View(model);
            }

            // Verifica se o nome de usuário já está sendo utilizado
            bool nomeUsuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.NomeUsuario == model.NomeUsuario);

            if (nomeUsuarioExiste)
            {
                ModelState.AddModelError(
                    "NomeUsuario",
                    "Este nome de usuário já está em uso."
                );

                return View(model);
            }

            // Cria o usuário que será salvo no banco
            var usuario = new Usuario
            {
                Nome = model.Nome,
                NomeUsuario = model.NomeUsuario,
                Email = model.Email,
                StatusConta = "Ativo",
                TipoUsuario = "Comum",
                DataCadastro = DateTime.Now
            };

            // Transforma a senha em hash antes de armazená-la
            var passwordHasher = new PasswordHasher<Usuario>();

            usuario.SenhaHash = passwordHasher.HashPassword(
                usuario,
                model.Senha
            );

            // Adiciona o usuário e salva no banco
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Depois do cadastro, envia o usuário para o login
            return RedirectToAction("Login");
        }

        // Temporário: depois criaremos a tela de login de verdade
        [HttpGet]
        public IActionResult Login()
        {
            return Content("Cadastro realizado! Aqui será a tela de login.");
        }
    }
}