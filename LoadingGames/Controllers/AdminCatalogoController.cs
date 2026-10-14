using LoadingGames.Data;
using LoadingGames.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Controllers;

public class AdminCatalogoController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminCatalogoController(ApplicationDbContext db) => _db = db;

    private bool EhAdministrador() =>
        HttpContext.Session.GetInt32("UsuarioId") != null &&
        HttpContext.Session.GetString("UsuarioTipo") == "Administrador";

    private static bool TipoValido(string? tipo) => tipo is "generos" or "plataformas" or "tags" or "desenvolvedoras" or "publicadoras";
    private static string Titulo(string tipo) => tipo switch
    {
        "generos" => "Gêneros",
        "plataformas" => "Plataformas",
        "desenvolvedoras" => "Desenvolvedoras",
        "publicadoras" => "Publicadoras",
        _ => "Tags"
    };

    [HttpGet]
    public async Task<IActionResult> Index(string tipo = "generos")
    {
        if (!EhAdministrador()) return StatusCode(403);
        if (!TipoValido(tipo)) return NotFound();

        var model = new AdminCatalogoViewModel { Tipo = tipo, Titulo = Titulo(tipo) };
        switch (tipo)
        {
            case "generos":
                model.Itens = await _db.Generos.AsNoTracking().OrderBy(g => g.Nome)
                    .Select(g => new AdminCatalogoItemViewModel
                    {
                        Id = g.IdGenero, Nome = g.Nome, Descricao = g.Descricao,
                        EmUso = g.Jogos.Any()
                    }).ToListAsync();
                break;
            case "plataformas":
                model.Itens = await _db.Plataformas.AsNoTracking().OrderBy(p => p.Nome)
                    .Select(p => new AdminCatalogoItemViewModel
                    {
                        Id = p.IdPlataforma, Nome = p.Nome, EmUso = p.Jogos.Any()
                    }).ToListAsync();
                break;
            case "desenvolvedoras":
                model.Itens = await _db.Desenvolvedores.AsNoTracking().OrderBy(d => d.Nome)
                    .Select(d => new AdminCatalogoItemViewModel
                    {
                        Id = d.IdDesenvolvedor, Nome = d.Nome, Descricao = d.Descricao,
                        EmUso = d.Jogos.Any()
                    }).ToListAsync();
                break;
            case "publicadoras":
                model.Itens = await _db.Publicadoras.AsNoTracking().OrderBy(p => p.Nome)
                    .Select(p => new AdminCatalogoItemViewModel
                    {
                        Id = p.IdPublicadora, Nome = p.Nome, Descricao = p.Descricao,
                        EmUso = p.Jogos.Any()
                    }).ToListAsync();
                break;
            case "tags":
                model.Itens = await _db.Tags.AsNoTracking().OrderBy(t => t.Nome)
                    .Select(t => new AdminCatalogoItemViewModel
                    {
                        Id = t.IdTag, Nome = t.Nome, EmUso = t.Jogos.Any()
                    }).ToListAsync();
                break;
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Formulario(string tipo = "generos", int? id = null)
    {
        if (!EhAdministrador()) return StatusCode(403);
        if (!TipoValido(tipo)) return NotFound();
        var model = new AdminCatalogoFormularioViewModel { Tipo = tipo };
        if (id.HasValue)
        {
            switch (tipo)
            {
                case "generos":
                    var g = await _db.Generos.AsNoTracking().FirstOrDefaultAsync(x => x.IdGenero == id);
                    if (g == null) return NotFound();
                    model.Id = g.IdGenero; model.Nome = g.Nome; model.Descricao = g.Descricao;
                    break;
                case "plataformas":
                    var p = await _db.Plataformas.AsNoTracking().FirstOrDefaultAsync(x => x.IdPlataforma == id);
                    if (p == null) return NotFound();
                    model.Id = p.IdPlataforma; model.Nome = p.Nome;
                    break;
                case "desenvolvedoras":
                    var d = await _db.Desenvolvedores.AsNoTracking().FirstOrDefaultAsync(x => x.IdDesenvolvedor == id);
                    if (d == null) return NotFound();
                    model.Id = d.IdDesenvolvedor; model.Nome = d.Nome; model.Descricao = d.Descricao;
                    break;
                case "publicadoras":
                    var pub = await _db.Publicadoras.AsNoTracking().FirstOrDefaultAsync(x => x.IdPublicadora == id);
                    if (pub == null) return NotFound();
                    model.Id = pub.IdPublicadora; model.Nome = pub.Nome; model.Descricao = pub.Descricao;
                    break;
                case "tags":
                    var t = await _db.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.IdTag == id);
                    if (t == null) return NotFound();
                    model.Id = t.IdTag; model.Nome = t.Nome;
                    break;
            }
        }
        ViewBag.Titulo = Titulo(tipo);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(AdminCatalogoFormularioViewModel model)
    {
        if (!EhAdministrador()) return StatusCode(403);
        if (!TipoValido(model.Tipo)) return NotFound();
        model.Nome = (model.Nome ?? "").Trim();
        model.Descricao = string.IsNullOrWhiteSpace(model.Descricao) ? null : model.Descricao.Trim();
        if (string.IsNullOrEmpty(model.Nome)) ModelState.AddModelError(nameof(model.Nome), "Informe o nome.");
        if (model.Tipo is not ("generos" or "desenvolvedoras" or "publicadoras")) model.Descricao = null;
        if (model.Tipo is "generos" or "plataformas" or "tags")
        {
            if (model.Nome.Length > 100) ModelState.AddModelError(nameof(model.Nome), "O nome pode ter até 100 caracteres.");
        }
        if (!ModelState.IsValid)
        {
            ViewBag.Titulo = Titulo(model.Tipo);
            return View("Formulario", model);
        }

        var nome = model.Nome;
        bool duplicado;
        switch (model.Tipo)
        {
            case "generos":
                duplicado = await _db.Generos.AnyAsync(g => g.Nome == nome && g.IdGenero != model.Id);
                break;
            case "plataformas":
                duplicado = await _db.Plataformas.AnyAsync(p => p.Nome == nome && p.IdPlataforma != model.Id);
                break;
            case "desenvolvedoras":
                duplicado = await _db.Desenvolvedores.AnyAsync(d => d.Nome == nome && d.IdDesenvolvedor != model.Id);
                break;
            case "publicadoras":
                duplicado = await _db.Publicadoras.AnyAsync(p => p.Nome == nome && p.IdPublicadora != model.Id);
                break;
            default:
                duplicado = await _db.Tags.AnyAsync(t => t.Nome == nome && t.IdTag != model.Id);
                break;
        }
        if (duplicado)
        {
            ModelState.AddModelError(nameof(model.Nome), "Já existe um cadastro com esse nome.");
            ViewBag.Titulo = Titulo(model.Tipo);
            return View("Formulario", model);
        }

        switch (model.Tipo)
        {
            case "generos":
                Genero? genero = model.Id.HasValue ? await _db.Generos.FindAsync(model.Id.Value) : new Genero();
                if (genero == null) return NotFound();
                genero.Nome = nome; genero.Descricao = model.Descricao;
                if (!model.Id.HasValue) _db.Generos.Add(genero);
                break;
            case "plataformas":
                Plataforma? plataforma = model.Id.HasValue ? await _db.Plataformas.FindAsync(model.Id.Value) : new Plataforma();
                if (plataforma == null) return NotFound();
                plataforma.Nome = nome;
                if (!model.Id.HasValue) _db.Plataformas.Add(plataforma);
                break;
            case "desenvolvedoras":
                Desenvolvedor? desenvolvedor = model.Id.HasValue ? await _db.Desenvolvedores.FindAsync(model.Id.Value) : new Desenvolvedor();
                if (desenvolvedor == null) return NotFound();
                desenvolvedor.Nome = nome; desenvolvedor.Descricao = model.Descricao;
                if (!model.Id.HasValue) _db.Desenvolvedores.Add(desenvolvedor);
                break;
            case "publicadoras":
                Publicadora? publicadora = model.Id.HasValue ? await _db.Publicadoras.FindAsync(model.Id.Value) : new Publicadora();
                if (publicadora == null) return NotFound();
                publicadora.Nome = nome; publicadora.Descricao = model.Descricao;
                if (!model.Id.HasValue) _db.Publicadoras.Add(publicadora);
                break;
            case "tags":
                Tag? tag = model.Id.HasValue ? await _db.Tags.FindAsync(model.Id.Value) : new Tag();
                if (tag == null) return NotFound();
                tag.Nome = nome;
                if (!model.Id.HasValue) _db.Tags.Add(tag);
                break;
        }
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.Nome), "Não foi possível salvar. Confira se o nome já existe.");
            ViewBag.Titulo = Titulo(model.Tipo);
            return View("Formulario", model);
        }
        TempData["Sucesso"] = "Cadastro salvo com sucesso!";
        return RedirectToAction(nameof(Index), new { tipo = model.Tipo });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string tipo, int id)
    {
        if (!EhAdministrador()) return StatusCode(403);
        if (!TipoValido(tipo)) return NotFound();
        switch (tipo)
        {
            case "generos":
                var g = await _db.Generos.Include(x => x.Jogos).FirstOrDefaultAsync(x => x.IdGenero == id);
                if (g == null) return NotFound();
                if (g.Jogos.Any()) { TempData["Erro"] = "Gênero associado a jogos. Remova as associações antes de excluir."; break; }
                _db.Generos.Remove(g);
                break;
            case "plataformas":
                var p = await _db.Plataformas.Include(x => x.Jogos).FirstOrDefaultAsync(x => x.IdPlataforma == id);
                if (p == null) return NotFound();
                if (p.Jogos.Any()) { TempData["Erro"] = "Plataforma associada a jogos. Remova as associações antes de excluir."; break; }
                _db.Plataformas.Remove(p);
                break;
            case "desenvolvedoras":
                var d = await _db.Desenvolvedores.FirstOrDefaultAsync(x => x.IdDesenvolvedor == id);
                if (d == null) return NotFound();
                if (await _db.Jogos.AnyAsync(j => j.IdDesenvolvedor == id))
                {
                    TempData["Erro"] = "Desenvolvedora associada a jogos. Remova as associações antes de excluir.";
                    break;
                }
                _db.Desenvolvedores.Remove(d);
                break;
            case "publicadoras":
                var pub = await _db.Publicadoras.FirstOrDefaultAsync(x => x.IdPublicadora == id);
                if (pub == null) return NotFound();
                if (await _db.Jogos.AnyAsync(j => j.IdPublicadora == id))
                {
                    TempData["Erro"] = "Publicadora associada a jogos. Remova as associações antes de excluir.";
                    break;
                }
                _db.Publicadoras.Remove(pub);
                break;
            case "tags":
                var t = await _db.Tags.Include(x => x.Jogos).FirstOrDefaultAsync(x => x.IdTag == id);
                if (t == null) return NotFound();
                if (t.Jogos.Any()) { TempData["Erro"] = "Tag associada a jogos. Remova as associações antes de excluir."; break; }
                _db.Tags.Remove(t);
                break;
        }
        if (TempData["Erro"] == null)
        {
            try
            {
                await _db.SaveChangesAsync();
                TempData["Sucesso"] = "Cadastro excluído com sucesso!";
            }
            catch (DbUpdateException)
            {
                TempData["Erro"] = "Não foi possível excluir: este cadastro está em uso.";
            }
        }
        return RedirectToAction(nameof(Index), new { tipo });
    }
}
