using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("publicadora")]
    public class Publicadora
    {
        [Key]
        [Column("id_publicadora")]
        public int IdPublicadora { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
    }
}