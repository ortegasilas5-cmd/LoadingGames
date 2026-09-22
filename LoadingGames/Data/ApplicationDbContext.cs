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

        // TABELAS

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Jogo> Jogos { get; set; }

        public DbSet<Genero> Generos { get; set; }

        public DbSet<Desenvolvedor> Desenvolvedores { get; set; }

        public DbSet<Publicadora> Publicadoras { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // JOGO -> DESENVOLVEDOR
            // =====================================================

            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Desenvolvedor)
                .WithMany(d => d.Jogos)
                .HasForeignKey(j => j.IdDesenvolvedor);


            // =====================================================
            // JOGO -> PUBLICADORA
            // =====================================================

            modelBuilder.Entity<Jogo>()
                .HasOne(j => j.Publicadora)
                .WithMany(p => p.Jogos)
                .HasForeignKey(j => j.IdPublicadora);


            // =====================================================
            // JOGO <-> GENERO
            // Relação muitos-para-muitos através de jogo_genero
            // =====================================================

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