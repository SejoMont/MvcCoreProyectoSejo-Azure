using Microsoft.AspNetCore.Authorization;
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
        private ServiceStorageBlobs serviceStorageBlobs;

        public ArtistasEventoController(ServiceEventos service, ServiceStorageBlobs serviceStorageBlobs)
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
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
        public async Task<IActionResult> CrearArtista(Artista artista, IFormFile file)
        {
            string blobName = file.FileName;

            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageBlobs.UploadBlobAsync("usuarios", blobName, stream);
            }

            artista.Foto = blobName;

            await this.service.CrearArtistaAsync(artista);
            return RedirectToAction("Details", "Eventos", new { id = artista.IdEvento });
        }

        [Authorize]
        [HttpPost("DeleteArtistaEvento")]
        public async Task<IActionResult> DeleteArtistaEvento(int idevento, int idartista)
        {
            bool success = await this.service.DeleteArtistaEventoAsync(idevento, idartista);
            if (success)
            {
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            else
            {
                ViewData["Error"] = "Error";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }

        [Authorize]
        [HttpPost("DeleteArtistaTemp")]
        public async Task<IActionResult> DeleteArtistaTemp(int idevento, int idartista)
        {
            bool success = await this.service.DeleteArtistaTempAsync(idevento, idartista);
            if (success)
            {
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            else
            {
                ViewData["Error"] = "Error";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }
    }
}
