using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("nome_usuario")]
        [MaxLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("senha_hash")]
        [MaxLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Column("foto_perfil_url")]
        [MaxLength(500)]
        public string? FotoPerfilUrl { get; set; }

        [Required]
        [Column("status_conta")]
        public string StatusConta { get; set; } = "Ativo";

        [Required]
        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; } = "Comum";

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}