using System.ComponentModel.DataAnnotations;
namespace LoadingGames.Models;
public class JogoFormularioViewModel
{
    public int IdJogo { get; set; }
    [Required(ErrorMessage = "Informe o nome."), MaxLength(150)] public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    [Range(typeof(decimal), "0", "99999999", ErrorMessage = "Informe um preço válido.")] public decimal Preco { get; set; }
    [MaxLength(500)] public string? ImagemPrincipalUrl { get; set; }
    [DataType(DataType.Date)] public DateTime? DataLancamento { get; set; }
    [RegularExpression("^(0|10|12|14|16|18)$", ErrorMessage = "Use 0, 10, 12, 14, 16 ou 18.")] public byte ClassificacaoIndicativa { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o desenvolvedor.")] public int IdDesenvolvedor { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Selecione a publicadora.")] public int IdPublicadora { get; set; }
    public bool Ativo { get; set; } = true;
    public List<int> GenerosIds { get; set; } = new();
    public List<int> TagsIds { get; set; } = new();
    public List<int> PlataformasIds { get; set; } = new();
    [Display(Name = "URLs das capturas de tela")] public string? ImagensUrls { get; set; }
}
