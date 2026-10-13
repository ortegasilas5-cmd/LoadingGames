using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoadingGames.Models;
[Table("plataforma")]
public class Plataforma
{
    [Key, Column("id_plataforma")] public int IdPlataforma { get; set; }
    [Required, MaxLength(100), Column("nome")] public string Nome { get; set; } = string.Empty;
    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}
