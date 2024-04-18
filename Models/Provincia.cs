using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("Provincias")]
    public class Provincia
    {
        [Key]
        [Column("ProvinciaID")]
        public int Id { get; set; }

        [Column("NombreProvincia")]
        [StringLength(100)]
        public string Nombre { get; set; }
    }
}
