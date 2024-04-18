using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Repository;

public class EventosController : Controller
{
    private EventosRepository repo;
    private UsuariosRepository userRepo;
    private ProvinciasRepository provinciasRepo;
    private EntradasRepository entradasRepo;
    private ArtistasEventoRepository artistsRepo;
    private HelperPathProvider helperPathProvider;
    private UploadFilesController uploadFilesController;

    public EventosController(EventosRepository repo, 
        UsuariosRepository userRepo, 
        ProvinciasRepository provinciasRepo, 
        EntradasRepository entradasRepo, 
        HelperPathProvider helperPathProvider, 
        UploadFilesController uploadFilesController, 
        ArtistasEventoRepository artistsRepo)
    {
        this.repo = repo;
        this.userRepo = userRepo;
        this.provinciasRepo = provinciasRepo;
        this.entradasRepo = entradasRepo;
        this.helperPathProvider = helperPathProvider;
        this.uploadFilesController = uploadFilesController;
        this.artistsRepo = artistsRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? iduser, [FromQuery] FiltroEvento filtro, int page = 1)
    {
        // Cantidad de eventos por página
        int pageSize = 8;

        List<EventoDetalles> eventos = new List<EventoDetalles>();
        List<TipoEvento> tipoEventos = await this.repo.GetTipoEventosAsync();
        List<Provincia> provincias = await this.provinciasRepo.GetAllProvinciassAsync();

        if (iduser != null)
        {
            UsuarioDetalles user = await this.userRepo.GetUsuarioDetalles(iduser ?? 0);
            ViewData["UsuarioDetalle"] = user;
        }

        if (filtro != null && filtro.TieneFiltros())
        {
            eventos = await this.repo.BuscarEventosPorFiltros(filtro);
        }
        else
        {
            eventos = await this.repo.GetAllEventosHoyAsync();
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
        List<EventoDetalles> eventos = await this.repo.GetAllEventosTipoAsync(tipo);
        List<TipoEvento> tipoEventos = await this.repo.GetTipoEventosAsync();

        ViewData["TipoEventos"] = tipoEventos;

        return View(eventos);
    }


    public async Task<IActionResult> Details(int id)
    {
        EventoDetalles eventoDetalles = await this.repo.GetDetallesEventoAsync(id);
        List<ArtistaDetalles> artistas = await this.userRepo.GetAllArtistasEventoAsync(id);
        List<ComentarioDetalles> comentarios = await this.repo.GetComentariosByEventoIdAsync(id);
        List<Artista> artistasEvento = await this.artistsRepo.GetArtistasTempAsync(id);

        ViewData["ArtistasUsers"] = artistas;
        ViewData["ArtistasEvento"] = artistasEvento;
        ViewData["Comentarios"] = comentarios;

        return View(eventoDetalles);
    }

    public IActionResult CrearEvento()
    {
        ViewData["Provincias"] = this.provinciasRepo.GetAllProvinciassAsync().Result;
        ViewData["TiposEventos"] = this.repo.GetTipoEventosAsync().Result;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CrearEvento(string NombreEvento, int TipoEventoID, DateTime Fecha, string Ubicacion, int Provincia, int Aforo, IFormFile Imagen, int Recinto, bool MayorDe18, string Descripcion, string LinkMapsProvincia, int Precio)
    {
        try
        {
            if (Imagen != null && Imagen.Length > 0)
            {
                // Llamar al método SubirFichero del controlador UploadFilesController
                await uploadFilesController.SubirFicheroEventos(Imagen);

                // Obtener la ruta completa del archivo utilizando el HelperPathProvider
                string nombreArchivo = Imagen.FileName;

                // Mapear el modelo de vista a la entidad de Evento
                var nuevoEvento = new Evento
                {
                    NombreEvento = NombreEvento,
                    Fecha = Fecha,
                    TipoEventoID = TipoEventoID,
                    Ubicacion = Ubicacion,
                    Provincia = Provincia,
                    Aforo = Aforo,
                    Imagen = nombreArchivo, 
                    Recinto = Recinto,
                    MayorDe18 = MayorDe18,
                    Descripcion = Descripcion,
                    LinkMapsProvincia = LinkMapsProvincia,
                    EntradasVendidas = 0,
                    Precio = Precio
                };

                // Llamar al repositorio para crear el evento (asincrónico)
                await this.repo.CrearEventoAsync(nuevoEvento);

                // Redirigir a la página de detalles del evento recién creado
                return RedirectToAction("Details", new { id = nuevoEvento.EventoID });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Por favor, seleccione una imagen.");
                ViewData["Provincias"] = await provinciasRepo.GetAllProvinciassAsync();
                ViewData["TiposEventos"] = await repo.GetTipoEventosAsync();
                return View("CrearEvento");
            }
        }
        catch (Exception ex)
        {
            // Manejar la excepción
            ModelState.AddModelError(string.Empty, "Error al procesar la solicitud. Por favor, inténtalo de nuevo.");
            return View("CrearEvento");
        }
    }

    public async Task<IActionResult> Comprar(int idevento)
    {
        EventoDetalles evento = await this.repo.GetDetallesEventoAsync(idevento);

        return View(evento);
    }
    [HttpPost]
    public async Task<IActionResult> Comprar(int nentradas, List<AsistenciaEvento> entradas)
    {

        foreach (var entrada in entradas)
        {
            await entradasRepo.AsignarEntradasAsync(entrada.EventoID, entrada.UsuarioID, entrada.Nombre, entrada.Correo, entrada.Dni);
            await repo.RestarEntrada(entrada.EventoID);
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

        await repo.AddComentarioAsync(comentario);
        return RedirectToAction("Details", new { id = eventoId });
    }

    
}
