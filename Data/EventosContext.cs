using Microsoft.EntityFrameworkCore;

namespace MvcCoreProyectoSejo.Models
{
    public class EventosContext : DbContext
    {
        public EventosContext(DbContextOptions<EventosContext> options)
            : base(options)
        {
        }

        // Agrega DbSet para cada entidad en tu modelo
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TipoEvento> TiposEventos { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<ArtistaEvento> ArtistasEvento { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ComentarioDetalles> ComentariosDetalles { get; set; }
        public DbSet<AsistenciaEvento> AsistenciasEventos { get; set; }
        public DbSet<Seguimiento> Seguimientos { get; set; }
        public DbSet<EventoDetalles> EventosDetalles { get; set; }
        public DbSet<UsuarioDetalles> UsuariosDetalles { get; set; }
        public DbSet<ArtistaDetalles> ArtistasDetalles { get; set; }
        public DbSet<EntradaDetalles> EntradaDetalles { get; set; }
        public DbSet<Artista> Artistas { get; set; }
    }
}
