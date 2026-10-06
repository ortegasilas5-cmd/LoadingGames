using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("carrinho")]
    public class Carrinho
    {
        [Key]
        [Column("id_carrinho")]
        public int IdCarrinho { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<ItemCarrinho> Itens { get; set; }
            = new List<ItemCarrinho>();
    }
}