using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("avaliacao")]
    public class Avaliacao
    {
        [Key]
        [Column("id_avaliacao")]
        public int IdAvaliacao { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_jogo")]
        public int IdJogo { get; set; }

        [Required]
        [Column("tipo_avaliacao")]
        [MaxLength(10)]
        public string Tipo { get; set; } = string.Empty;

        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("data_avaliacao")]
        public DateTime DataAvaliacao { get; set; }

        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(IdJogo))]
        public Jogo Jogo { get; set; } = null!;
    }
}