using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("compra")]
    public class Compra
    {
        [Key]
        [Column("id_compra")]
        public int IdCompra { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("data_compra")]
        public DateTime DataCompra { get; set; }

        [Column("valor_total")]
        public decimal ValorTotal { get; set; }

        [Required]
        [Column("forma_pagamento")]
        [MaxLength(20)]
        public string FormaPagamento { get; set; } = string.Empty;

        [Required]
        [Column("status_compra")]
        [MaxLength(20)]
        public string StatusCompra { get; set; } = string.Empty;

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<ItemCompra> Itens { get; set; }
            = new List<ItemCompra>();

        public ICollection<PosseJogo> Posses { get; set; }
            = new List<PosseJogo>();
    }
}