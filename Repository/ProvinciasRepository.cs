using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Models;

namespace MvcCoreProyectoSejo.Repository
{
    public class ProvinciasRepository
    {
        private EventosContext context;

        public ProvinciasRepository(EventosContext context)
        {
            this.context = context;
        }
        public async Task<List<Provincia>> GetAllProvinciassAsync()
        {
            var provincias = await this.context.Provincias.ToListAsync();

            return provincias;
        }

    }
}
