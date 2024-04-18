using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    [Table("VISTA_ENTRADAS_DETALLE")]
    public class EntradaDetalles
    {
        [Key]
        [Column("AsistenciaID")]
        public int AsistenciaID { get; set; }

        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("EventoID")]
        public int EventoID { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("DNI")]
        public string Dni { get; set; }

        [Column("QR")]
        public string QR { get; set; }

        [Column("NombreEvento")]
        public string NombreEvento { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Provincia")]
        public string Provincia { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        [Column("Recinto")]
        public string Recinto { get; set; }
    }
}
