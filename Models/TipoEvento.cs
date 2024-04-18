using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("TiposEventos")]
    public class TipoEvento
    {
        [Key]
        [Column("TipoEventoID")]
        public int Id { get; set; }

        [Column("Tipo")]
        public string Nombre { get; set; }
        [Column("Imagen")]
        public string Imagen { get; set; }
    }
}
