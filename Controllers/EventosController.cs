using System.Linq;
using ApiCoreProyectoEventos.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;

public class EventosController : Controller
{
    private ServiceEventos service;

    public EventosController(ServiceEventos service)
    {

        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? iduser, [FromQuery] FiltroEvento filtro, int page = 1)
    {
        // Cantidad de eventos por página
        int pageSize = 8;

        List<EventoDetalles> eventos = await this.service.GetEventosAsync();
        List<TipoEvento> tipoEventos = await this.service.GetTipoEventosAsync();
        List<Provincia> provincias = await this.service.GetProvinciasAsync();

        if (iduser != null)
        {
            UsuarioDetalles user = await this.service.GetUsuarioDetallesAsync(iduser ?? 0);
            ViewData["UsuarioDetalle"] = user;
        }

        if (filtro != null && filtro.TieneFiltros())
        {
            eventos = await this.service.GetEventosPorFiltrosAsync(filtro);
        }
        else
        {
            eventos = await this.service.GetEventosAsync();
        }

        // Paginar la lista de eventos
        var model = eventos.Skip((page - 1) * pageSize).Take(pageSize);

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
    public async Task<IActionResult> CrearEvento(Evento evento)
    {
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


}
