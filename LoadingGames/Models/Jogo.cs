using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("jogo")]
    public class Jogo
    {
        [Key]
        [Column("id_jogo")]
        public int IdJogo { get; set; }

        [Column("id_desenvolvedor")]
        public int IdDesenvolvedor { get; set; }

        [Column("id_publicadora")]
        public int IdPublicadora { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("preco")]
        public decimal Preco { get; set; }

        [Column("imagem_principal_url")]
        [MaxLength(500)]
        public string? ImagemPrincipalUrl { get; set; }

        [Column("data_lancamento")]
        public DateTime? DataLancamento { get; set; }

        [Column("classificacao_indicativa")]
        public byte ClassificacaoIndicativa { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }


        // RELACIONAMENTOS

        [ForeignKey(nameof(IdDesenvolvedor))]
        public Desenvolvedor Desenvolvedor { get; set; } = null!;

        [ForeignKey(nameof(IdPublicadora))]
        public Publicadora Publicadora { get; set; } = null!;

        public ICollection<Genero> Generos { get; set; } = new List<Genero>();
    }
}