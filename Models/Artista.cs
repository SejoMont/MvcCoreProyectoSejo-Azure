using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MvcCoreProyectoSejo.Models
{
    [Table("Artistas")]
    public class Artista
    {
        [Key]
        [Column("idArtista")]
        public int IdArtista { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }
        [Column("Foto")]
        public string Foto { get; set; }
        [Column("IdEvento")]
        public int IdEvento { get; set; }
    }
}
