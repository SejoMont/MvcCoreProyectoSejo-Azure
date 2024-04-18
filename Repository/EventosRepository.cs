using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Models;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Text.Json.Nodes;
#region VISTAS Y PROCEDURES
//ALTER VIEW VISTA_DETALLES_EVENTO AS
//SELECT
//    E.EventoID,
//  E.NombreEvento,
//  TE.Tipo AS TipoEvento,
//  E.Fecha,
//  E.Ubicacion,
//  P.NombreProvincia AS Provincia,
//  P.ProvinciaID,
//  E.Aforo,
//  E.EntradasVendidas,
//  E.Imagen,
//  U.NombreUsuario AS Recinto,
//  U.UsuarioID AS RecintoID,
//  E.MayorDe18,
//  E.Descripcion,
//  E.Precio,
//  E.LinkMapsProvincia
//FROM Eventos E
//JOIN TiposEventos TE ON E.TipoEventoID = TE.TipoEventoID
//JOIN Provincias P ON E.Provincia = P.ProvinciaID
//JOIN Usuarios U ON E.Recinto = U.UsuarioID;
#endregion

namespace MvcCoreProyectoSejo.Repository
{
    public class EventosRepository
    {
        private EventosContext context;

        public EventosRepository(EventosContext context)
        {
            this.context = context;
        }

        public async Task<List<EventoDetalles>> GetAllEventosHoyAsync()
        {
            // Obtener la fecha de hoy
            DateTime fechaHoy = DateTime.Today;

            // Filtrar los eventos que ocurran a partir de hoy y ordenarlos por fecha
            var eventos = await this.context.EventosDetalles
                .Where(e => e.Fecha >= fechaHoy)
                .OrderBy(e => e.Fecha)
                .ToListAsync();

            return eventos;
        }


        public async Task<List<EventoDetalles>> GetAllEventosAsync()
        {
            var eventos = await this.context.EventosDetalles.ToListAsync();

            return eventos;
        }

        public async Task<EventoDetalles> GetDetallesEventoAsync(int idevento)
        {
            return await this.context.EventosDetalles.FirstOrDefaultAsync(z => z.Id == idevento);
        }

        public async Task<List<TipoEvento>> GetTipoEventosAsync()
        {
            var tiposevento = await this.context.TiposEventos.ToListAsync();

            return tiposevento;
        }

        public async Task<List<EventoDetalles>> GetAllEventosTipoAsync(string tipo)
        {
            var eventos = await this.context.EventosDetalles
                .Where(a => a.TipoEvento == tipo)
                .ToListAsync();

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetAllEventosArtistaAsync(int idartista)
        {
            var eventos = await this.context.EventosDetalles
                .Where(e => this.context.ArtistasEvento.Any(ae => ae.ArtistaID == idartista && ae.EventoID == e.Id))
                .ToListAsync();

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetEventosPorRecintoAsync(int idRecinto)
        {
            return await this.context.EventosDetalles
                .Where(e => e.RecintoId == idRecinto)
                .ToListAsync();
        }

        public async Task<List<EventoDetalles>> GetAllEventosProvinciasAsync(int idprovincia)
        {
            var eventos = await this.context.EventosDetalles
                .Where(a => a.ProvinciaID == idprovincia)
                .ToListAsync();

            return eventos;
        }

        public async Task CrearEventoAsync(Evento evento)
        {
            context.Eventos.Add(evento);
            await context.SaveChangesAsync();
        }

        public async Task RestarEntrada(int idevento)
        {
            // Obtener el evento por su ID

            Evento evento = await this.context.Eventos.FirstOrDefaultAsync(e => e.EventoID == idevento);

            // Verificar si se encontró el evento
            if (evento != null)
            {
                // Restar una entrada
                evento.EntradasVendidas++;

                // Guardar los cambios en la base de datos
                await context.SaveChangesAsync();
            }
            else
            {
                // Manejar el caso cuando el evento no se encuentra
                throw new InvalidOperationException("El evento no existe.");
            }
        }

        public async Task BorrarEvento(int idevento)
        {
            // Buscar el evento por su ID
            var evento = await this.context.Eventos.FindAsync(idevento);

            // Eliminar el evento de la base de datos
            this.context.Eventos.Remove(evento);
        }

        public async Task AddComentarioAsync(Comentario resena)
        {
            context.Comentarios.Add(resena);
            await context.SaveChangesAsync();
        }

        public async Task<List<ComentarioDetalles>> GetComentariosByEventoIdAsync(int eventoId)
        {
            return await context.ComentariosDetalles
                                 .Where(r => r.EventoID == eventoId)
                                 .Include(r => r.Usuario)
                                 .ToListAsync();
        }

        public async Task<IActionResult> BuscarArtistas(string term)
        {
            var artistas = await context.ArtistasDetalles
                              .Where(a => a.NombreUsuario.Contains(term))
                              .Select(a => new { label = a.NombreUsuario, value = a.UsuarioID })
                              .ToListAsync();

            return new JsonResult(artistas);
        }


        public async Task<List<EventoDetalles>> BuscarEventosPorFiltros(FiltroEvento filtro)
        {
            IQueryable<EventoDetalles> query = this.context.EventosDetalles;

            if (!string.IsNullOrEmpty(filtro.Nombre))
            {
                query = query.Where(e => e.NombreEvento.Contains(filtro.Nombre));
            }

            if (filtro.FechaInicio.HasValue)
            {
                query = query.Where(e => e.Fecha == filtro.FechaInicio);
            }

            if (!string.IsNullOrEmpty(filtro.Provincia))
            {
                query = query.Where(e => e.Provincia == filtro.Provincia);
            }

            if (!string.IsNullOrEmpty(filtro.Tipo))
            {
                query = query.Where(e => e.TipoEvento == filtro.Tipo);
            }

            if (filtro.PrecioMayorQue.HasValue)
            {
                query = query.Where(e => e.Precio > filtro.PrecioMayorQue);
            }

            if (filtro.PrecioMenorQue.HasValue)
            {
                query = query.Where(e => e.Precio < filtro.PrecioMenorQue);
            }

            return await query.ToListAsync();
        }
    }
}
