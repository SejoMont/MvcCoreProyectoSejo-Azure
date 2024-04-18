using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    [Table("VISTA_DETALLES_EVENTO")]
    public class EventoDetalles
    {
        [Column("EventoID")]
        public int Id { get; set; }

        [Column("NombreEvento")]
        [StringLength(100)]
        [Required]
        public string NombreEvento { get; set; }

        [Column("TipoEvento")]
        public string TipoEvento { get; set; }

        [Column("Fecha")]
        [Required]
        public DateTime Fecha { get; set; }

        [Column("Ubicacion")]
        public string Ubicacion { get; set; }

        [Column("Provincia")]
        public string Provincia { get; set; }
        [Column("ProvinciaID")]
        public int ProvinciaID { get; set; }

        [Column("Aforo")]
        [Required]
        public int Aforo { get; set; }
        [Column("EntradasVendidas")]
        public int EntradasVendidas { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        [Column("Recinto")]
        public string Recinto { get; set; }
        [Column("RecintoID")]
        public int RecintoId { get; set; }

        [Column("MayorDe18")]
        [Required]
        public bool MayorDe18 { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Precio")]
        public int Precio { get; set; }

        [Column("LinkMapsProvincia")]
        public string LinkMapsProvincia { get; set; }
    }
}
