using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("desenvolvedor")]
    public class Desenvolvedor
    {
        [Key]
        [Column("id_desenvolvedor")]
        public int IdDesenvolvedor { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
    }
}