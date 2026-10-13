using LoadingGames.Data;
using LoadingGames.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuração da conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(
		connectionString,
		ServerVersion.AutoDetect(connectionString)
	)
);

// ==========================================
// CONFIGURAÇÃO DA SESSION
// ==========================================
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// ==========================================
// ATIVA A SESSION
// ==========================================
app.UseSession();

// Revalida o status e o tipo do usuario em cada requisicao.
app.UseMiddleware<ValidacaoSessaoMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.Run();