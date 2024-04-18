using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MvcCoreProyectoSejo.Helpers;
using System.IO;

public class UploadFilesController : Controller
{
    private HelperPathProvider helperPathProvider;

    public UploadFilesController(HelperPathProvider helperPathProvider)
    {
        this.helperPathProvider = helperPathProvider;
    }

    [HttpPost]
    public async Task<IActionResult> SubirFicheroEventos(IFormFile fichero)
    {
        try
        {
            if (fichero != null && fichero.Length > 0)
            {
                // Obtener la ruta completa del archivo utilizando HelperPathProvider
                string path = this.helperPathProvider.MapPath(fichero.FileName, Folders.Eventos);

                // Subir el archivo utilizando Stream
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    // Copiar el contenido del archivo al stream
                    await fichero.CopyToAsync(stream);
                }

                return View();
            }
            else
            {
                // Manejar el caso en el que no se proporciona ningún archivo
                ViewData["MENSAJE"] = "No se proporcionó ningún archivo para subir.";
                return View();
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción que ocurra durante el proceso de subida de archivos
            ViewData["MENSAJE"] = "Error al subir el archivo: " + ex.Message;
            return View();
        }
    }
    [HttpPost]
    public async Task<IActionResult> SubirFicheroUsuarios(IFormFile fichero)
    {
        try
        {
            if (fichero != null && fichero.Length > 0)
            {
                // Obtener la ruta completa del archivo utilizando HelperPathProvider
                string path = this.helperPathProvider.MapPath(fichero.FileName, Folders.Usuarios);

                // Subir el archivo utilizando Stream
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    // Copiar el contenido del archivo al stream
                    await fichero.CopyToAsync(stream);
                }

                return View();
            }
            else
            {
                // Manejar el caso en el que no se proporciona ningún archivo
                ViewData["MENSAJE"] = "No se proporcionó ningún archivo para subir.";
                return View();
            }
        }
        catch (Exception ex)
        {
            // Manejar cualquier excepción que ocurra durante el proceso de subida de archivos
            ViewData["MENSAJE"] = "Error al subir el archivo: " + ex.Message;
            return View();
        }
    }
}
