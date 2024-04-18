using Microsoft.AspNetCore.Mvc;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Repository;

namespace MvcCoreProyectoSejo.Controllers
{
    public class EntradasController : Controller
    {
        private EntradasRepository repo;

        public EntradasController(EntradasRepository repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> VerEntradas(int iduser)
        {
            List<EntradaDetalles> entradasUsuario = await this.repo.GetAllEntradasUsuarioAsync(iduser);

            // Agrupar las entradas por EventoID y contar el número de entradas en cada grupo
            //List<EntradasPorEventoViewModel> entradasAgrupadas = entradasUsuario.GroupBy(e => e.EventoID)
            //                                        .Select(g => new EntradasPorEventoViewModel
            //                                        {
            //                                            EventoID = g.Key,
            //                                            NumeroDeEntradas = g.Count(),
            //                                            Entradas = g.ToList()
            //                                        })
            //                                        .ToList();

            return View(entradasUsuario);
        }
    }
}
