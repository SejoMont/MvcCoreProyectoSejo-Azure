using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Repository;

namespace MvcCoreProyectoSejo.Controllers
{
    public class ArtistasEventoController : Controller
    {
        private ArtistasEventoRepository repo;
        private EventosRepository eventosRepository;
        private UsuariosRepository usuariosRepository;
        private EventosContext context;
        private HelperPathProvider helperPathProvider;
        private UploadFilesController uploadFilesController;

        public ArtistasEventoController(ArtistasEventoRepository repo, EventosRepository eventosRepository, UsuariosRepository usuariosRepository, EventosContext context, HelperPathProvider helperPathProvider, UploadFilesController uploadFilesController)
        {
            this.repo = repo;
            this.eventosRepository = eventosRepository;
            this.usuariosRepository = usuariosRepository;
            this.context = context;
            this.helperPathProvider = helperPathProvider;
            this.uploadFilesController = uploadFilesController;
        }
        public async Task<IActionResult> _AddArtistaToEvento(int idevento)
        {
            EventoDetalles evento = await this.eventosRepository.GetDetallesEventoAsync(idevento);
            List<UsuarioDetalles> artistas = await this.usuariosRepository.GetAllArtistas();

            ViewData["Evento"] = evento;

            return PartialView("_AddArtistaToEvento", artistas);
        }

        [HttpPost]
        public async Task<IActionResult> _AddArtistaToEvento(int idevento, int idartista)
        {
            try
            {
                await this.repo.AddArtistaToEvento(idevento, idartista);
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
            catch (Exception ex)
            {
                TempData["ERROR"] = "El Artista ya esta en este Evento";
                return RedirectToAction("Details", "Eventos", new { id = idevento });
            }
        }

        public async Task<JsonResult> BuscarArtistas(string term)
        {
            var artistas = await this.context.ArtistasDetalles
                                  .Where(a => a.NombreUsuario.Contains(term))
                                  .Select(a => new { label = a.NombreUsuario, value = a.UsuarioID })
                                  .ToListAsync();

            return Json(artistas);
        }

        [HttpPost]
        public async Task<IActionResult> CrearArtista(Artista artista, IFormFile foto)
        {
            // Llamar al método SubirFichero del controlador UploadFilesController
            await uploadFilesController.SubirFicheroUsuarios(foto);

            artista.Foto = foto.FileName;

            this.context.Artistas.Add(artista);
            await this.context.SaveChangesAsync();

            return RedirectToAction("Details", "Eventos", new { id = artista.IdEvento });
        }
    }
}
