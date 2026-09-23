using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("genero")]
    public class Genero
    {
        [Key]
        [Column("id_genero")]
        public int IdGenero { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [MaxLength(255)]
        public string? Descricao { get; set; }

        public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
    }
}