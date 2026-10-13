using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoadingGames.Models;
[Table("tag")]
public class Tag
{
    [Key, Column("id_tag")] public int IdTag { get; set; }
    [Required, MaxLength(100), Column("nome")] public string Nome { get; set; } = string.Empty;
    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}
