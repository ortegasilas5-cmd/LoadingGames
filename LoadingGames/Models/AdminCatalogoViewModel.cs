using System.ComponentModel.DataAnnotations;

namespace LoadingGames.Models;

public class AdminCatalogoViewModel
{
    public string Tipo { get; set; } = "generos";
    public string Titulo { get; set; } = "";
    public List<AdminCatalogoItemViewModel> Itens { get; set; } = new();
}

public class AdminCatalogoItemViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public bool EmUso { get; set; }
}

public class AdminCatalogoFormularioViewModel
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Informe o nome.")]
    [StringLength(100, ErrorMessage = "O nome pode ter até 100 caracteres.")]
    public string Nome { get; set; } = "";
    [StringLength(255, ErrorMessage = "A descrição pode ter até 255 caracteres.")]
    public string? Descricao { get; set; }
    public string Tipo { get; set; } = "generos";
}
