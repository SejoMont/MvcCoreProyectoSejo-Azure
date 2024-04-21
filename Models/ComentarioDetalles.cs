using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("VISTA_DETALLE_COMENTARIO")]
    public class ComentarioDetalles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ComentarioID")]
        public int ComentarioID { get; set; }

        [Column("EventoID")]
        public int EventoID { get; set; }

        [Column("NombreEvento")]
        public string NombreEvento { get; set; }

        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("NombreUsuario")]
        public string NombreUsuario { get; set; }

        [Column("Texto")]
        public string Texto { get; set; }

        [Column("Puntuacion")]
        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("EventoID")]
        public Evento Evento { get; set; }
    }
}
