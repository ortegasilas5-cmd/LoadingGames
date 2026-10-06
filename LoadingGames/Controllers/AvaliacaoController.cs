using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(
            int idJogo,
            string tipo,
            string? comentario)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            bool jogoExiste = await _context.Jogos
                .AnyAsync(j =>
                    j.IdJogo == idJogo &&
                    j.Ativo);

            if (!jogoExiste)
            {
                return NotFound();
            }

            bool possuiJogo = await _context.PossesJogos
                .AnyAsync(p =>
                    p.IdUsuario == usuarioId.Value &&
                    p.IdJogo == idJogo);

            if (!possuiJogo)
            {
                TempData["Aviso"] =
                    "Você só pode avaliar jogos que possui.";

                return RedirectToAction(
                    "Details",
                    "Jogo",
                    new { id = idJogo }
                );
            }

            bool jaAvaliou = await _context.Avaliacoes
                .AnyAsync(a =>
                    a.IdUsuario == usuarioId.Value &&
                    a.IdJogo == idJogo);

            if (jaAvaliou)
            {
                TempData["Aviso"] =
                    "Você já avaliou este jogo.";

                return RedirectToAction(
                    "Details",
                    "Jogo",
                    new { id = idJogo }
                );
            }

            string[] tiposPermitidos =
            {
                "Positiva",
                "Negativa"
            };

            if (string.IsNullOrWhiteSpace(tipo) ||
                !tiposPermitidos.Contains(tipo))
            {
                TempData["Aviso"] =
                    "Selecione uma avaliação válida.";

                return RedirectToAction(
                    "Details",
                    "Jogo",
                    new { id = idJogo }
                );
            }

            comentario = comentario?.Trim();

            if (string.IsNullOrWhiteSpace(comentario))
            {
                comentario = null;
            }

            var avaliacao = new Avaliacao
            {
                IdUsuario = usuarioId.Value,
                IdJogo = idJogo,
                Tipo = tipo,
                Comentario = comentario,
                DataAvaliacao = DateTime.Now
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Avaliação publicada com sucesso!";

            return RedirectToAction(
                "Details",
                "Jogo",
                new { id = idJogo }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int idAvaliacao,
            string tipo,
            string? comentario)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var avaliacao = await _context.Avaliacoes
                .FirstOrDefaultAsync(a =>
                    a.IdAvaliacao == idAvaliacao &&
                    a.IdUsuario == usuarioId.Value);

            if (avaliacao == null)
            {
                return NotFound();
            }

            string[] tiposPermitidos =
            {
                "Positiva",
                "Negativa"
            };

            if (string.IsNullOrWhiteSpace(tipo) ||
                !tiposPermitidos.Contains(tipo))
            {
                TempData["Aviso"] =
                    "Selecione uma avaliação válida.";

                return RedirectToAction(
                    "Details",
                    "Jogo",
                    new { id = avaliacao.IdJogo }
                );
            }

            comentario = comentario?.Trim();

            if (string.IsNullOrWhiteSpace(comentario))
            {
                comentario = null;
            }

            avaliacao.Tipo = tipo;
            avaliacao.Comentario = comentario;
            avaliacao.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Avaliação atualizada com sucesso!";

            return RedirectToAction(
                "Details",
                "Jogo",
                new { id = avaliacao.IdJogo }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int idAvaliacao)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var avaliacao = await _context.Avaliacoes
                .FirstOrDefaultAsync(a =>
                    a.IdAvaliacao == idAvaliacao &&
                    a.IdUsuario == usuarioId.Value);

            if (avaliacao == null)
            {
                return NotFound();
            }

            int idJogo = avaliacao.IdJogo;

            _context.Avaliacoes.Remove(avaliacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] =
                "Avaliação excluída com sucesso!";

            return RedirectToAction(
                "Details",
                "Jogo",
                new { id = idJogo }
            );
        }
    }
}