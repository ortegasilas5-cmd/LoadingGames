using LoadingGames.Data;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Middleware
{
    public class ValidacaoSessaoMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidacaoSessaoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            int? id = context.Session.GetInt32("UsuarioId");

            if (id.HasValue)
            {
                var usuario = await db.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IdUsuario == id.Value)
                    .Select(u => new
                    {
                        u.Nome,
                        u.Email,
                        u.TipoUsuario,
                        u.StatusConta
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null || usuario.StatusConta != "Ativo")
                {
                    context.Session.Clear();
                    context.Response.Redirect("/Login/Index?bloqueado=1");
                    return;
                }

                // Nao confia em permissoes antigas armazenadas na sessao.
                context.Session.SetString("UsuarioTipo", usuario.TipoUsuario);
                context.Session.SetString("UsuarioNome", usuario.Nome);
                context.Session.SetString("UsuarioEmail", usuario.Email);
            }

            await _next(context);
        }
    }
}
