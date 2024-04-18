using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("Seguimientos")]
    public class Seguimiento
    {
        [Key]
        [Column("SeguimientoID")]
        public int SeguimientoID { get; set; }

        [Column("UsuarioIDSeguidor")]
        public int UsuarioIDSeguidor { get; set; }

        [ForeignKey("UsuarioIDSeguidor")]
        public Usuario UsuarioSeguidor { get; set; }

        [Column("UsuarioIDSeguido")]
        public int UsuarioIDSeguido { get; set; }

        [ForeignKey("UsuarioIDSeguido")]
        public Usuario UsuarioSeguido { get; set; }
    }

}
