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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Plataforma> Plataformas { get; set; }
        public DbSet<ImagemJogo> ImagensJogos { get; set; }
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

            modelBuilder.Entity<Jogo>()
                .HasMany(j => j.Tags)
                .WithMany(t => t.Jogos)
                .UsingEntity<Dictionary<string, object>>(
                    "jogo_tag",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("id_tag"),
                    l => l.HasOne<Jogo>().WithMany().HasForeignKey("id_jogo"),
                    j => j.HasKey("id_jogo", "id_tag"));

            modelBuilder.Entity<Jogo>()
                .HasMany(j => j.Plataformas)
                .WithMany(p => p.Jogos)
                .UsingEntity<Dictionary<string, object>>(
                    "jogo_plataforma",
                    r => r.HasOne<Plataforma>().WithMany().HasForeignKey("id_plataforma"),
                    l => l.HasOne<Jogo>().WithMany().HasForeignKey("id_jogo"),
                    j => j.HasKey("id_jogo", "id_plataforma"));

            modelBuilder.Entity<ImagemJogo>()
                .HasOne(i => i.Jogo)
                .WithMany(j => j.Imagens)
                .HasForeignKey(i => i.IdJogo);
        }
    }
}