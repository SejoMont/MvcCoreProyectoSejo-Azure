using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCoreProyectoSejo.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("Usuarios")]
    public class Usuario
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

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("RolID")]
        public int RolID { get; set; }

        [Column("Password")]
        public byte[] Password { get; set; }

        [Column("Salt")]
        public string Salt { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }

        [Column("TokenMail")]
        public string TokenMail { get; set; }

        [ForeignKey("ProvinciaID")]
        public Provincia ProvinciaUsuario { get; set; }

        [ForeignKey("RolID")]
        public Rol RolUsuario { get; set; }

    }
}