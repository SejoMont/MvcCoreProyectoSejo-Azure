using System.Linq;
using ApiCoreProyectoEventos.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;

public class EventosController : Controller
{
    private ServiceEventos service;
    private ServiceStorageBlobs serviceStorageBlobs;

    public EventosController(ServiceEventos service, ServiceStorageBlobs serviceStorageBlobs)
    {
        this.service = service;
        this.serviceStorageBlobs = serviceStorageBlobs;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? iduser, [FromQuery] FiltroEvento filtro, int page = 1)
    {
        // Cantidad de eventos por página
        int pageSize = 8;

        List<EventoDetalles> eventos = new List<EventoDetalles>(); // Inicializar una lista vacía

        if (filtro == null || !filtro.TieneFiltros())
        {
            eventos = (await this.service.GetEventosAsync()).Where(e => e.Fecha.Date >= DateTime.Today).ToList();
        }
        else
        {
            eventos = await this.service.GetEventosPorFiltrosAsync(filtro);
        }

        eventos = eventos.OrderBy(e => e.Fecha).ToList();

        // Paginar la lista de eventos
        var model = eventos.Skip((page - 1) * pageSize).Take(pageSize);

        List<TipoEvento> tipoEventos = await this.service.GetTipoEventosAsync();
        List<Provincia> provincias = await this.service.GetProvinciasAsync();

        if (iduser != null)
        {
            UsuarioDetalles user = await this.service.GetUsuarioDetallesAsync(iduser ?? 0);
            ViewData["UsuarioDetalle"] = user;
        }

        ViewData["TipoEventos"] = tipoEventos;
        ViewData["Provincias"] = provincias;

        // Agregar información de paginación a la vista
        ViewBag.PageNumber = page;
        ViewBag.TotalPages = Math.Ceiling((double)eventos.Count / pageSize);

        // Construir la cadena de consulta para los filtros
        string filtersQueryString = string.Empty;
        if (filtro != null)
        {
            var queryParameters = new Dictionary<string, string>
        {
            { "nombre", filtro.Nombre },
            { "fechaInicio", filtro.FechaInicio.HasValue ? filtro.FechaInicio.Value.ToString("yyyy-MM-dd") : "" },
            { "provincia", filtro.Provincia },
            { "tipo", filtro.Tipo },
            { "precioMayorQue", filtro.PrecioMayorQue.HasValue ? filtro.PrecioMayorQue.Value.ToString() : "" },
            { "precioMenorQue", filtro.PrecioMenorQue.HasValue ? filtro.PrecioMenorQue.Value.ToString() : "" }
        };
            filtersQueryString = QueryHelpers.AddQueryString("", queryParameters);
        }

        // Agregar la cadena de consulta a la vista
        ViewBag.FiltersQueryString = filtersQueryString;

        return View(model);
    }


    public async Task<IActionResult> TipoEvento(string tipo)
    {
        List<EventoDetalles> eventos = await this.service.GetAllEventosTipoAsync(tipo);
        List<TipoEvento> tipoEventos = await this.service.GetTipoEventosAsync();

        ViewData["TipoEventos"] = tipoEventos;

        return View(eventos);
    }


    public async Task<IActionResult> Details(int id)
    {
        EventoDetalles eventoDetalles = await this.service.FindEventoAsync(id);
        List<ArtistaDetalles> artistas = await this.service.GetAllArtistasEventoAsync(id);
        List<ComentarioDetalles> comentarios = await this.service.GetComentariosEventoAsync(id);
        List<Artista> artistasEvento = await this.service.GetAllArtistasTempEventoAsync(id);

        ViewData["ArtistasUsers"] = artistas;
        ViewData["ArtistasEvento"] = artistasEvento;
        ViewData["Comentarios"] = comentarios;


        return View(eventoDetalles);
    }

    [AuthorizeRoles("3")]
    public IActionResult CrearEvento()
    {
        ViewData["Provincias"] = this.service.GetProvinciasAsync().Result;
        ViewData["TiposEventos"] = this.service.GetTipoEventosAsync().Result;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearEvento(Evento evento, IFormFile file)
    {

        string blobName = file.FileName;

        using (Stream stream = file.OpenReadStream())
        {
            await this.serviceStorageBlobs.UploadBlobAsync("eventos", blobName, stream);
        }

        evento.Imagen = blobName;

        Evento createdEvento = await service.CrearEventoAsync(evento);
        if (createdEvento != null)
        {
            // Retorna una respuesta 201 Created con la ubicación del nuevo recurso.
            return RedirectToAction("Details", "Eventos", new { id = createdEvento.EventoID });
        }
        else
        {
            // Si la creación falla, considera devolver un código de error adecuado.
            return StatusCode(500, "No se pudo crear el evento");
        }
    }

    public async Task<IActionResult> Comprar(int idevento)
    {
        EventoDetalles evento = await this.service.FindEventoAsync(idevento);

        return View(evento);
    }
    [HttpPost]
    public async Task<IActionResult> Comprar(int nentradas, List<AsistenciaEvento> entradas)
    {

        foreach (var entrada in entradas)
        {
            await this.service.AsignarEntradasAsync(entrada);
            await this.service.RestarEntradaAsync(entrada.EventoID);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddComentario(int eventoId, string texto, int userId, int puntuacion)
    {
        var comentario = new Comentario
        {
            EventoID = eventoId,
            UsuarioID = userId,
            Texto = texto,
            FechaCreacion = DateTime.Now,
            Puntuacion = puntuacion
        };

        await this.service.AddComentarioAsync(comentario);
        return RedirectToAction("Details", new { id = eventoId });
    }

    [Authorize]
    [HttpPost("DeleteComentario")]
    public async Task<IActionResult> DeleteComentario(int idcoment)
    {
        bool success = await this.service.DeleteComentarioAsync(idcoment);
        if (success)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewData["Error"] = "Error";
            return RedirectToAction("Index");
        }
    }
}
