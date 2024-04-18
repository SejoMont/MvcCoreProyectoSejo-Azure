using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    public class AsistenciaEvento
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
    }
}
