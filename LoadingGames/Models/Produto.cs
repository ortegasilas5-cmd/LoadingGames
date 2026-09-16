using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("produto")]
    public class Produto
    {
        [Key]
        [Column("id_produto")]
        public int IdProduto { get; set; }

        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("id_fabricante")]
        public int IdFabricante { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("preco")]
        public decimal Preco { get; set; }

        [Column("codigo_barras")]
        public string? CodigoBarras { get; set; }

        [Column("imagem_url")]
        public string ImagemUrl { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }
    }
}