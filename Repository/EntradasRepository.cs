using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Models;

#region VISTAS
//CREATE VIEW VISTA_ENTRADAS_DETALLE AS
//SELECT 
//    AE.AsistenciaID,
//    AE.UsuarioID,
//    AE.EventoID,
//    AE.Nombre,
//    AE.Correo,
//    AE.DNI,
//    AE.QR,
//    DE.NombreEvento,
//    DE.Fecha,
//    DE.Provincia,
//    DE.Imagen,
//    DE.Recinto
//FROM 
//    AsistenciasEventos AE
//JOIN 
//    VISTA_DETALLES_EVENTO DE ON AE.EventoID = DE.EventoID;
#endregion

namespace MvcCoreProyectoSejo.Repository
{
    public class EntradasRepository
    {
        private EventosContext context;

        public EntradasRepository(EventosContext context)
        {
            this.context = context;
        }
        public async Task AsignarEntradasAsync(int idevento, int iduser, string nombre, string correo, string dni)
        {
            AsistenciaEvento nuevaEntrada = new AsistenciaEvento()
            {
                UsuarioID = iduser,
                EventoID = idevento,
                Nombre = nombre,
                Correo = correo,
                Dni = dni,
                QR = ""
            };
            context.AsistenciasEventos.Add(nuevaEntrada);
        }

        public async Task<List<EntradaDetalles>> GetAllEntradasUsuarioAsync(int iduser)
        {
            var entradas = await this.context.EntradaDetalles
                .Where(u => u.UsuarioID == iduser)
                .ToListAsync();

            return entradas;
        }
    }
}
