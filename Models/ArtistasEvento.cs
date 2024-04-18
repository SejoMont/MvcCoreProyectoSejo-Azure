using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcCoreProyectoSejo.Models
{
    [Table("ArtistasEvento")]
    public class ArtistaEvento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ArtistaEventoID")]
        public int Id { get; set; }

        [Column("ArtistaID")]
        public int ArtistaID { get; set; }

        [Column("EventoID")]
        public int EventoID { get; set; }

        [ForeignKey("ArtistaID")]
        public Usuario Artista { get; set; }

        [ForeignKey("EventoID")]
        public Evento Evento { get; set; }
    }
}
