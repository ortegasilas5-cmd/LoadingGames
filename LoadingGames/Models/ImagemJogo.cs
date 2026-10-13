using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LoadingGames.Models;
[Table("imagem_jogo")]
public class ImagemJogo
{
    [Key, Column("id_imagem")] public int IdImagem { get; set; }
    [Column("id_jogo")] public int IdJogo { get; set; }
    [Required, MaxLength(500), Column("url_imagem")] public string UrlImagem { get; set; } = string.Empty;
    [Required, Column("tipo")] public string Tipo { get; set; } = "Screenshot";
    [ForeignKey(nameof(IdJogo))] public Jogo Jogo { get; set; } = null!;
}
