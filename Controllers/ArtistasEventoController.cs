using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;

namespace MvcCoreProyectoSejo.Controllers
{
    public class ArtistasEventoController : Controller
    {
        private ServiceEventos service;

        public ArtistasEventoController(ServiceEventos service)
        {
            this.service = service;

        }
        public async Task<IActionResult> _AddArtistaToEvento(int idevento)
        {
            EventoDetalles evento = await this.service.FindEventoAsync(idevento);
            List<UsuarioDetalles> artistas = await this.service.GetAllArtistasAsync();

            ViewData["Evento"] = evento;

            return PartialView("_AddArtistaToEvento", artistas);
        }

        [HttpPost]
        public async Task<IActionResult> _AddArtistaToEvento(int idevento, int idartista)
        {
            try
            {
                await this.service.AddArtistaEventoAsync(idevento, idartista);
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            catch (Exception ex)
            {
                TempData["ERROR"] = "El Artista ya esta en este Evento";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CrearArtista(Artista artista)
        {
            await this.service.CrearArtistaAsync(artista);
            return RedirectToAction("Details", "Eventos", new { id = artista.IdEvento });
        }
    }
}
