using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers
{
    public class JogoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public JogoController(ApplicationDbContext context) => _context = context;
        private bool EhAdministrador() => HttpContext.Session.GetInt32("UsuarioId") != null &&
            HttpContext.Session.GetString("UsuarioTipo") == "Administrador";

        [HttpGet]
        public async Task<IActionResult> Index(string? pesquisa, int? genero, int? plataforma, int? tag)
        {
            var jogos = _context.Jogos.AsNoTracking()
                .Include(j => j.Desenvolvedor).Include(j => j.Publicadora)
                .Include(j => j.Generos).Include(j => j.Plataformas).Include(j => j.Tags)
                .Where(j => j.Ativo).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim();
                var termos = pesquisa.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var termo in termos)
                {
                    var palavra = termo;
                    jogos = jogos.Where(j => j.Nome.Contains(palavra));
                }
            }
            if (genero.HasValue) jogos = jogos.Where(j => j.Generos.Any(g => g.IdGenero == genero.Value));
            if (plataforma.HasValue) jogos = jogos.Where(j => j.Plataformas.Any(p => p.IdPlataforma == plataforma.Value));
            if (tag.HasValue) jogos = jogos.Where(j => j.Tags.Any(t => t.IdTag == tag.Value));
            await CarregarListas();
            ViewBag.Pesquisa = pesquisa;
            ViewBag.GeneroSelecionado = genero;
            ViewBag.PlataformaSelecionada = plataforma;
            ViewBag.TagSelecionada = tag;
            return View(await jogos.OrderBy(j => j.Nome).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var jogo = await _context.Jogos.AsNoTracking()
                .Include(j => j.Desenvolvedor).Include(j => j.Publicadora)
                .Include(j => j.Generos).Include(j => j.Tags).Include(j => j.Plataformas)
                .Include(j => j.Imagens).FirstOrDefaultAsync(j => j.IdJogo == id);
            if (jogo == null) return NotFound();
            bool possuiJogo = usuarioId.HasValue && await _context.PossesJogos
                .AnyAsync(p => p.IdUsuario == usuarioId.Value && p.IdJogo == id);
            if (!jogo.Ativo && !possuiJogo) return NotFound();
            ViewBag.Avaliacoes = await _context.Avaliacoes.Include(a => a.Usuario)
                .Where(a => a.IdJogo == id).OrderByDescending(a => a.DataAvaliacao).ToListAsync();
            ViewBag.PossuiJogo = possuiJogo;
            ViewBag.AvaliacaoUsuario = usuarioId.HasValue
                ? await _context.Avaliacoes.FirstOrDefaultAsync(a => a.IdUsuario == usuarioId.Value && a.IdJogo == id)
                : null;
            return View(jogo);
        }

        [HttpGet]
        public async Task<IActionResult> Crud()
        {
            if (!EhAdministrador()) return StatusCode(403);
            return View(await _context.Jogos.Include(j => j.Desenvolvedor)
                .Include(j => j.Publicadora).OrderBy(j => j.Nome).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!EhAdministrador()) return StatusCode(403);
            await CarregarListas();
            return View(new JogoFormularioViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JogoFormularioViewModel form)
        {
            if (!EhAdministrador()) return StatusCode(403);
            await ValidarFormulario(form);
            if (!ModelState.IsValid) { await CarregarListas(); return View(form); }
            var jogo = new Jogo();
            AplicarCampos(jogo, form);
            await AplicarRelacionamentos(jogo, form);
            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Jogo cadastrado com sucesso!";
            return RedirectToAction(nameof(Crud));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EhAdministrador()) return StatusCode(403);
            var jogo = await _context.Jogos.Include(j => j.Generos).Include(j => j.Tags)
                .Include(j => j.Plataformas).Include(j => j.Imagens)
                .FirstOrDefaultAsync(j => j.IdJogo == id);
            if (jogo == null) return NotFound();
            await CarregarListas();
            return View(new JogoFormularioViewModel
            {
                IdJogo = jogo.IdJogo, Nome = jogo.Nome, Descricao = jogo.Descricao,
                Preco = jogo.Preco, ImagemPrincipalUrl = jogo.ImagemPrincipalUrl,
                DataLancamento = jogo.DataLancamento, ClassificacaoIndicativa = jogo.ClassificacaoIndicativa,
                IdDesenvolvedor = jogo.IdDesenvolvedor, IdPublicadora = jogo.IdPublicadora,
                Ativo = jogo.Ativo, GenerosIds = jogo.Generos.Select(g => g.IdGenero).ToList(),
                TagsIds = jogo.Tags.Select(t => t.IdTag).ToList(),
                PlataformasIds = jogo.Plataformas.Select(p => p.IdPlataforma).ToList(),
                ImagensUrls = string.Join(Environment.NewLine, jogo.Imagens.OrderBy(i => i.IdImagem).Select(i => i.UrlImagem))
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JogoFormularioViewModel form)
        {
            if (!EhAdministrador()) return StatusCode(403);
            if (id != form.IdJogo) return NotFound();
            var jogo = await _context.Jogos.Include(j => j.Generos).Include(j => j.Tags)
                .Include(j => j.Plataformas).Include(j => j.Imagens)
                .FirstOrDefaultAsync(j => j.IdJogo == id);
            if (jogo == null) return NotFound();
            await ValidarFormulario(form);
            if (!ModelState.IsValid) { await CarregarListas(); return View(form); }
            AplicarCampos(jogo, form);
            await AplicarRelacionamentos(jogo, form);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Jogo atualizado com sucesso!";
            return RedirectToAction(nameof(Crud));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AlterarStatus(int id)
        {
            if (!EhAdministrador()) return StatusCode(403);
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null) return NotFound();
            jogo.Ativo = !jogo.Ativo;
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = jogo.Ativo ? $"O jogo '{jogo.Nome}' foi reativado com sucesso!"
                : $"O jogo '{jogo.Nome}' foi desativado com sucesso!";
            return RedirectToAction(nameof(Crud));
        }

        private static void AplicarCampos(Jogo jogo, JogoFormularioViewModel form)
        {
            jogo.Nome = form.Nome.Trim();
            jogo.Descricao = form.Descricao;
            jogo.Preco = form.Preco;
            jogo.ImagemPrincipalUrl = form.ImagemPrincipalUrl?.Trim();
            jogo.DataLancamento = form.DataLancamento;
            jogo.ClassificacaoIndicativa = form.ClassificacaoIndicativa;
            jogo.IdDesenvolvedor = form.IdDesenvolvedor;
            jogo.IdPublicadora = form.IdPublicadora;
            jogo.Ativo = form.Ativo;
        }

        private static List<string> ExtrairUrls(string? texto) => (texto ?? "")
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        private async Task ValidarFormulario(JogoFormularioViewModel form)
        {
            if (!await _context.Desenvolvedores.AnyAsync(d => d.IdDesenvolvedor == form.IdDesenvolvedor))
                ModelState.AddModelError(nameof(form.IdDesenvolvedor), "Desenvolvedor inválido.");
            if (!await _context.Publicadoras.AnyAsync(p => p.IdPublicadora == form.IdPublicadora))
                ModelState.AddModelError(nameof(form.IdPublicadora), "Publicadora inválida.");
            if (form.GenerosIds.Distinct().Count() != await _context.Generos.CountAsync(g => form.GenerosIds.Contains(g.IdGenero)))
                ModelState.AddModelError(nameof(form.GenerosIds), "Seleção de gêneros inválida.");
            if (form.TagsIds.Distinct().Count() != await _context.Tags.CountAsync(t => form.TagsIds.Contains(t.IdTag)))
                ModelState.AddModelError(nameof(form.TagsIds), "Seleção de tags inválida.");
            if (form.PlataformasIds.Distinct().Count() != await _context.Plataformas.CountAsync(p => form.PlataformasIds.Contains(p.IdPlataforma)))
                ModelState.AddModelError(nameof(form.PlataformasIds), "Seleção de plataformas inválida.");
            if (ExtrairUrls(form.ImagensUrls).Any(url => url.Length > 500 || !UrlValida(url)))
                ModelState.AddModelError(nameof(form.ImagensUrls), "Use URLs http/https válidas, uma por linha, com até 500 caracteres.");
            if (!string.IsNullOrWhiteSpace(form.ImagemPrincipalUrl) && !UrlValida(form.ImagemPrincipalUrl))
                ModelState.AddModelError(nameof(form.ImagemPrincipalUrl), "Informe uma URL http/https válida.");
        }

        private static bool UrlValida(string url) => Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        private async Task AplicarRelacionamentos(Jogo jogo, JogoFormularioViewModel form)
        {
            var generos = await _context.Generos.Where(g => form.GenerosIds.Contains(g.IdGenero)).ToListAsync();
            var tags = await _context.Tags.Where(t => form.TagsIds.Contains(t.IdTag)).ToListAsync();
            var plataformas = await _context.Plataformas.Where(p => form.PlataformasIds.Contains(p.IdPlataforma)).ToListAsync();
            jogo.Generos.Clear(); foreach (var genero in generos) jogo.Generos.Add(genero);
            jogo.Tags.Clear(); foreach (var tag in tags) jogo.Tags.Add(tag);
            jogo.Plataformas.Clear(); foreach (var plataforma in plataformas) jogo.Plataformas.Add(plataforma);
            var urls = ExtrairUrls(form.ImagensUrls);
            foreach (var antiga in jogo.Imagens.ToList())
            {
                if (!urls.Contains(antiga.UrlImagem, StringComparer.OrdinalIgnoreCase))
                {
                    jogo.Imagens.Remove(antiga);
                    _context.ImagensJogos.Remove(antiga);
                }
            }
            var existentes = jogo.Imagens.Select(i => i.UrlImagem).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var url in urls)
                if (!existentes.Contains(url)) jogo.Imagens.Add(new ImagemJogo { UrlImagem = url, Tipo = "Screenshot" });
        }

        private async Task CarregarListas()
        {
            ViewBag.Desenvolvedores = await _context.Desenvolvedores.OrderBy(d => d.Nome).ToListAsync();
            ViewBag.Publicadoras = await _context.Publicadoras.OrderBy(p => p.Nome).ToListAsync();
            ViewBag.Generos = await _context.Generos.OrderBy(g => g.Nome).ToListAsync();
            ViewBag.Tags = await _context.Tags.OrderBy(t => t.Nome).ToListAsync();
            ViewBag.Plataformas = await _context.Plataformas.OrderBy(p => p.Nome).ToListAsync();
        }
    }
}
