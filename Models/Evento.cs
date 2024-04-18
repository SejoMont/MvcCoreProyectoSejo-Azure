using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    [Table("Eventos")]
    public class Evento
    {
        [Key]
        [Column("EventoID")]
        public int EventoID { get; set; }

        [Column("NombreEvento")]
        [Required]
        public string NombreEvento { get; set; }

        [Column("TipoEventoID")]
        public int TipoEventoID { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Ubicacion")]
        public string Ubicacion { get; set; }

        [Column("Provincia")]
        public int Provincia { get; set; }

        [Column("Aforo")]
        public int Aforo { get; set; }

        [Column("Imagen")]
        public string Imagen { get; set; }

        [Column("Recinto")]
        public int Recinto { get; set; }

        [Column("MayorDe18")]
        public bool MayorDe18 { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("LinkMapsProvincia")]
        public string LinkMapsProvincia { get; set; }

        [Column("EntradasVendidas")]
        public int EntradasVendidas { get; set; }
        [Column("Precio")]
        public int Precio { get; set; }
    }
}
