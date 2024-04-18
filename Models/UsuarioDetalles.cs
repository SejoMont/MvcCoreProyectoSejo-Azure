using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    [Table("VISTA_DETALLE_USUARIO")]
    public class UsuarioDetalles
    {
        [Key]
        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("NombreUsuario")]
        [Required]
        public string NombreUsuario { get; set; }

        [Column("FotoPerfil")]
        public string FotoPerfil { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; }

        [Column("ProvinciaID")]
        public int ProvinciaID { get; set; }

        [Column("NombreProvincia")]
        public string NombreProvincia { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("NombreRol")]
        public string NombreRol { get; set; }
        [Column("RolID")]
        public int RolID { get; set; }

    }
}
