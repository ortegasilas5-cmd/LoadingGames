
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadingGames.Models
{
    [Table("fabricante")]
    public class Fabricante
    {
        [Key]
        [Column("id_fabricante")]
        public int IdFabricante { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("pais")]
        public string Pais { get; set; }
    }
}
 
