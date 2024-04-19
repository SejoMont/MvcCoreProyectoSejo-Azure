using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("Comentarios")]
    public class Comentario
    {
        [Key]
        [Column("ComentarioID")]
        public int ComentarioID { get; set; }

        [Column("EventoID")]
        public int EventoID { get; set; }

        [Column("UsuarioID")]
        public int UsuarioID { get; set; }

        [Column("Texto")]
        public string Texto { get; set; }

        [Column("Puntuacion")]
        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }
    }

}
