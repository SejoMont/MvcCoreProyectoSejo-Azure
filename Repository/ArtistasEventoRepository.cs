using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcCoreProyectoSejo.Models;

namespace MvcCoreProyectoSejo.Repository
{
    public class ArtistasEventoRepository
    {
        private EventosContext context;

        public ArtistasEventoRepository(EventosContext context)
        {
            this.context = context;
        }

        public async Task AddArtistaToEvento(int idevento, int idartista)
        {
            var existeArtistaEnEvento = await this.context.ArtistasEvento
                .AnyAsync(ae => ae.EventoID == idevento && ae.ArtistaID == idartista);

            if (!existeArtistaEnEvento)
            {
                ArtistaEvento artistaEvento = new ArtistaEvento
                {
                    EventoID = idevento,
                    ArtistaID = idartista
                };
                this.context.ArtistasEvento.Add(artistaEvento);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Artista>> GetArtistasTempAsync(int idevento)
        {
            return await context.Artistas
                                 .Where(r => r.IdEvento == idevento)
                                 .ToListAsync();
        }

    }
}
