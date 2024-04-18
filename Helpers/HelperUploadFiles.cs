using MvcCoreProyectoSejo.Helpers;

namespace MvcCoreProyectoSejo.Helpers
{
    // Clase que proporciona métodos para gestionar la subida de archivos
    public class HelperUploadFiles
    {
        private readonly HelperPathProvider helperPathProvider;

        // Constructor que recibe un HelperPathProvider para gestionar rutas y URLs
        public HelperUploadFiles(HelperPathProvider helperPathProvider)
        {
            this.helperPathProvider = helperPathProvider;
        }

        // Método para subir un archivo de forma asíncrona y devolver la ruta completa del archivo
        public async Task<string> UploadFileAsync(IFormFile file, Folders folder)
        {
            // Obtener el nombre del archivo y la ruta completa utilizando HelperPathProvider
            string fileName = file.FileName;
            string path = this.helperPathProvider.MapUrlPath(fileName, folder);

            // Copiar el contenido del archivo al stream
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Devolver la ruta completa del archivo
            return path;
        }
    }
}