using MvcCoreProyectoSejo.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MvcCoreProyectoSejo.Services
{
    public class ServiceEventos
    {
        private string urlApiEventos;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor httpContextAccessor;

        public ServiceEventos(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.urlApiEventos = configuration.GetValue<string>("ConnectionStrings:ApiEventos");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> CallApiSync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        #region Usuarios
        public async Task<string> Login(string correo, string pass)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Usuarios/Login";
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                Login login = new Login
                {
                    Correo = correo,
                    Password = pass
                };

                string jsonData = JsonConvert.SerializeObject(login);

                StringContent content =
                    new StringContent(jsonData, Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await
                    client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Usuario> GetUserAsync(string correo)
        {
            string request = "api/Usuarios/GetUser/" + correo;

            Usuario user = await this.CallApiSync<Usuario>(request);

            return user;
        }

        public async Task<UsuarioDetalles> GetUsuarioDetallesAsync(int iduser)
        {
            string request = "api/Usuarios/Details/" + iduser;

            UsuarioDetalles user = await this.CallApiSync<UsuarioDetalles>(request);

            return user;
        }
        #endregion

        #region Provincias
        public async Task<List<Provincia>> GetProvinciasAsync()
        {
            string request = "api/Provincias/GetAllProvincias";

            List<Provincia> provincias = await this.CallApiSync<List<Provincia>>(request);

            return provincias;
        }
        #endregion

        #region Eventos
        public async Task<List<EventoDetalles>> GetEventosAsync()
        {
            string request = "/api/Eventos/GetEventos";

            List<EventoDetalles> eventos = await this.CallApiSync<List<EventoDetalles>>(request);

            return eventos;
        }

        public async Task<Evento> CrearEventoAsync(Evento evento)
        {
            string requestUri = "api/Eventos/CrearEvento";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string jsonData = JsonConvert.SerializeObject(evento);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    // Deserializa y devuelve el objeto Evento creado
                    string responseData = await response.Content.ReadAsStringAsync();
                    Evento createdEvento = JsonConvert.DeserializeObject<Evento>(responseData);
                    return createdEvento;
                }
                else
                {
                    // Considera manejar diferentes tipos de errores o devolver un valor nulo
                    return null;
                }
            }
        }

        public async Task<List<EventoDetalles>> GetAllEventosTipoAsync(string tipo)
        {
            string request = "/api/Eventos/GetAllEventosTipo?tipo=" + tipo;

            List<EventoDetalles> eventos = await this.CallApiSync<List<EventoDetalles>>(request);

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetEventosPorFiltrosAsync(FiltroEvento filtro)
        {
            StringBuilder queryString = new StringBuilder("api/Eventos/GetEventosPorFiltros?");

            if (!string.IsNullOrEmpty(filtro.Nombre))
            {
                queryString.Append($"nombre={Uri.EscapeDataString(filtro.Nombre)}&");
            }
            if (filtro.FechaInicio.HasValue)
            {
                queryString.Append($"fechaInicio={filtro.FechaInicio.Value.ToString("yyyy-MM-dd")}&");
            }
            if (!string.IsNullOrEmpty(filtro.Provincia))
            {
                queryString.Append($"provincia={Uri.EscapeDataString(filtro.Provincia)}&");
            }
            if (!string.IsNullOrEmpty(filtro.Tipo))
            {
                queryString.Append($"tipo={Uri.EscapeDataString(filtro.Tipo)}&");
            }
            if (filtro.PrecioMayorQue.HasValue)
            {
                queryString.Append($"precioMayorQue={filtro.PrecioMayorQue.Value}&");
            }
            if (filtro.PrecioMenorQue.HasValue)
            {
                queryString.Append($"precioMenorQue={filtro.PrecioMenorQue.Value}&");
            }

            // Elimina el último '&' si está presente
            string requestUrl = queryString.ToString().TrimEnd('&');

            return await CallApiSync<List<EventoDetalles>>(requestUrl);
        }


        public async Task<List<TipoEvento>> GetTipoEventosAsync()
        {
            string request = "api/Eventos/GetTipoEventos";

            List<TipoEvento> tipoEventos = await this.CallApiSync<List<TipoEvento>>(request);

            return tipoEventos;
        }

        public async Task<EventoDetalles> FindEventoAsync(int idevento)
        {
            string request = "api/Eventos/FindEvento?id=" + idevento;

            EventoDetalles evento = await this.CallApiSync<EventoDetalles>(request);

            return evento;
        }
        #endregion

        #region Artistas
        public async Task<List<Artista>> GetAllArtistasTempEventoAsync(int idevento)
        {
            string request = "api/ArtistasEvento/GetArtistasTempEvento/?idevento=" + idevento;

            List<Artista> artistas = await this.CallApiSync<List<Artista>>(request);

            return artistas;
        }

        public async Task<List<ArtistaDetalles>> GetAllArtistasEventoAsync(int idevento)
        {
            string request = "api/ArtistasEvento/GetArtistasEvento/?idevento=" + idevento;

            List<ArtistaDetalles> artistas = await this.CallApiSync<List<ArtistaDetalles>>(request);

            return artistas;
        }
        #endregion

        #region Comentarios
        public async Task<bool> AddComentarioAsync(Comentario comentario)
        {
            string request = "api/Comentarios/AddComentario";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string jsonData = JsonConvert.SerializeObject(comentario);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<List<ComentarioDetalles>> GetComentariosEventoAsync(int idevento)
        {
            string request = "/api/Comentarios/GetComentariosEvento?idevento=" + idevento;

            List<ComentarioDetalles> comentarios = await this.CallApiSync<List<ComentarioDetalles>>(request);

            return comentarios;
        }
        #endregion

        #region Entradas
        public async Task<bool> AsignarEntradasAsync(AsistenciaEvento entrada)
        {
            string request = "api/Entradas/AsignarEntrada";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string jsonData = JsonConvert.SerializeObject(entrada);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> RestarEntradaAsync(int idevento)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Construcción del endpoint para restar una entrada
                string requestUri = $"api/Entradas/RestarEntrada/{idevento}";

                // Realizar la petición POST sin un cuerpo, ya que solo necesitamos el ID del evento
                HttpResponseMessage response = await client.PostAsync(requestUri, null);

                // Verificar si la operación fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Podrías manejar diferentes tipos de errores aquí
                    return false;
                }
            }
        }

        #endregion
    }
}
