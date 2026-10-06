using LoadingGames.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingGames.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Desenvolvedor> Desenvolvedores { get; set; }
        public DbSet<Publicadora> Publicadoras { get; set; }

        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<ItemCarrinho> ItensCarrinho { get; set; }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItensCompra { get; set; }
        public DbSet<PosseJogo> PossesJogos { get; set; }

        public DbSet<Avaliacao> Avaliacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Desenvolvedor)
                .WithMany(d => d.Jogos)
                .HasForeignKey(j => j.IdDesenvolvedor);

            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Publicadora)
                .WithMany(p => p.Jogos)
                .HasForeignKey(j => j.IdPublicadora);

            modelBuilder.Entity<Jogo>()
                .HasMany(j => j.Generos)
                .WithMany(g => g.Jogos)
                .UsingEntity<Dictionary<string, object>>(
                    "jogo_genero",
                    j => j
                        .HasOne<Genero>()
                        .WithMany()
                        .HasForeignKey("id_genero"),
                    j => j
                        .HasOne<Jogo>()
                        .WithMany()
                        .HasForeignKey("id_jogo")
                );
        }
    }
}