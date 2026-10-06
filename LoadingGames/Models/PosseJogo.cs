using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("posse_jogo")]
    public class PosseJogo
    {
        [Key]
        [Column("id_posse")]
        public int IdPosse { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_jogo")]
        public int IdJogo { get; set; }

        [Column("id_compra")]
        public int IdCompra { get; set; }

        [Column("data_aquisicao")]
        public DateTime DataAquisicao { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(IdJogo))]
        public Jogo Jogo { get; set; } = null!;

        [ForeignKey(nameof(IdCompra))]
        public Compra Compra { get; set; } = null!;
    }
}