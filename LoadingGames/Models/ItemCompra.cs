using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("item_compra")]
    public class ItemCompra
    {
        [Key]
        [Column("id_item_compra")]
        public int IdItemCompra { get; set; }

        [Column("id_compra")]
        public int IdCompra { get; set; }

        [Column("id_jogo")]
        public int IdJogo { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [ForeignKey(nameof(IdCompra))]
        public Compra Compra { get; set; } = null!;

        [ForeignKey(nameof(IdJogo))]
        public Jogo Jogo { get; set; } = null!;
    }
}