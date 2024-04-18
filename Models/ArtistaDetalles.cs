using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("VISTA_DETALLE_ARTISTA")]
    public class ArtistaDetalles
    {
        [Key]
        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("NombreUsuario")]
        [Required]
        public string NombreUsuario { get; set; }

        [Column("FotoPerfil")]
        public string FotoPerfil { get; set; }

        [Column("ProvinciaID")]
        public int ProvinciaID { get; set; }

        [Column("NombreProvincia")]
        public string NombreProvincia { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("RolID")]
        public int RolID { get; set; }

        [Column("NombreRol")]
        public string NombreRol { get; set; }

        [Column("EventoID")]
        public int EventoID { get; set; }
    }
}
