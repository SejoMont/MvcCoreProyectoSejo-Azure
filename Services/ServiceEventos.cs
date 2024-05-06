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

        #region Metodos Genericos
        //    public async Task<string> GetTokenAsync(string username
        //, string password)
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            string request = "api/Usuarios/login";
        //            client.BaseAddress = new Uri(this.urlApiEventos);
        //            client.DefaultRequestHeaders.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(this.header);
        //            Login model = new Login
        //            {
        //                Correo = username,
        //                Password = password
        //            };
        //            string jsonData = JsonConvert.SerializeObject(model);
        //            StringContent content =
        //                new StringContent(jsonData, Encoding.UTF8,
        //                "application/json");
        //            HttpResponseMessage response = await
        //                client.PostAsync(request, content);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string data = await response.Content.ReadAsStringAsync();
        //                JObject keys = JObject.Parse(data);
        //                string token = keys.GetValue("response").ToString();
        //                return token;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
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

        //TENDREMOS UN METODO GENERICO QUE RECIBIRA EL REQUEST 
        //Y EL TOKEN
        private async Task<T> CallApiAsync<T>
            (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
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

        #endregion

        #region Usuarios
        public async Task<string> Login(Login login)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/Usuarios/Login";
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

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

        public async Task<bool> EmailExists(string correo)
        {
            string request = "api/Usuarios/EmailExists?correo=" + correo;

            bool emailExist = await this.CallApiAsync<bool>(request);

            return emailExist;
        }

        public async Task<Usuario> RegisterUserAsync(Registro registro)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = "api/Usuarios/Registro";
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string jsonData = JsonConvert.SerializeObject(registro);

                StringContent content =
                    new StringContent(jsonData, Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Usuario usuario = JsonConvert.DeserializeObject<Usuario>(responseContent);
                    return usuario;
                }
                else
                {
                    // Considerar manejar diferentes tipos de errores o estados de respuesta aquí
                    return null;
                }
            }
        }

        public async Task<Usuario> GetUserAsync(string correo)
        {
            string request = "api/Usuarios/GetUser/" + correo;

            Usuario user = await this.CallApiAsync<Usuario>(request);

            return user;
        }

        public async Task<UsuarioDetalles> GetUsuarioDetallesAsync(int iduser)
        {
            string request = "api/Usuarios/Details/" + iduser;

            UsuarioDetalles user = await this.CallApiAsync<UsuarioDetalles>(request);

            return user;
        }
        #endregion

        #region Provincias
        public async Task<List<Provincia>> GetProvinciasAsync()
        {
            string request = "api/Provincias/GetAllProvincias";

            List<Provincia> provincias = await this.CallApiAsync<List<Provincia>>(request);

            return provincias;
        }
        #endregion

        #region Eventos
        public async Task<List<EventoDetalles>> GetEventosAsync()
        {
            string request = "/api/Eventos/GetEventos";

            List<EventoDetalles> eventos = await this.CallApiAsync<List<EventoDetalles>>(request);

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

                // Recuperar el token JWT de la sesión
                string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

                // Añadir el token JWT en el encabezado Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string jsonData = JsonConvert.SerializeObject(evento);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Evento createdEvento = JsonConvert.DeserializeObject<Evento>(responseData);
                    return createdEvento;
                }
                else
                {
                    // Manejo de errores, posiblemente devolver excepciones específicas
                    return null;
                }
            }
        }



        public async Task<List<EventoDetalles>> GetAllEventosTipoAsync(string tipo)
        {
            string request = "api/Eventos/GetAllEventosTipo?tipo=" + tipo;

            List<EventoDetalles> eventos = await this.CallApiAsync<List<EventoDetalles>>(request);

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetAllEventosArtistaAsync(int iduser)
        {
            string request = "api/Eventos/GetAllEventosArtista?iduser=" + iduser;

            List<EventoDetalles> eventos = await this.CallApiAsync<List<EventoDetalles>>(request);

            return eventos;
        }

        public async Task<List<EventoDetalles>> GetEventosPorRecintoAsync(int iduser)
        {
            string request = "api/Eventos/GetEventosPorRecinto?iduser=" + iduser;

            List<EventoDetalles> eventos = await this.CallApiAsync<List<EventoDetalles>>(request);

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

            return await CallApiAsync<List<EventoDetalles>>(requestUrl);
        }


        public async Task<List<TipoEvento>> GetTipoEventosAsync()
        {
            string request = "api/Eventos/GetTipoEventos";

            List<TipoEvento> tipoEventos = await this.CallApiAsync<List<TipoEvento>>(request);

            return tipoEventos;
        }

        public async Task<EventoDetalles> FindEventoAsync(int idevento)
        {
            string request = "api/Eventos/FindEvento?id=" + idevento;

            EventoDetalles evento = await this.CallApiAsync<EventoDetalles>(request);

            return evento;
        }
        #endregion

        #region Artistas
        public async Task<List<Artista>> GetAllArtistasTempEventoAsync(int idevento)
        {
            string request = "api/ArtistasEvento/GetArtistasTempEvento/?idevento=" + idevento;

            List<Artista> artistas = await this.CallApiAsync<List<Artista>>(request);

            return artistas;
        }

        public async Task<List<ArtistaDetalles>> GetAllArtistasEventoAsync(int idevento)
        {
            string request = "api/ArtistasEvento/GetArtistasEvento/?idevento=" + idevento;

            List<ArtistaDetalles> artistas = await this.CallApiAsync<List<ArtistaDetalles>>(request);

            return artistas;
        }

        public async Task<List<UsuarioDetalles>> GetAllArtistasAsync()
        {
            string request = "api/ArtistasEvento/GetAllArtistas";

            List<UsuarioDetalles> artistas = await this.CallApiAsync<List<UsuarioDetalles>>(request);

            return artistas;
        }

        public async Task<bool> AddArtistaEventoAsync(int idevento, int idartista)
        {
            string request = "api/ArtistasEvento/AddArtistaToEvento/" + idevento + "/" + idartista;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.PostAsync(request, null);

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

        public async Task<bool> CrearArtistaAsync(Artista artista)
        {
            string request = "api/ArtistasEvento/CrearArtista";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string jsonData = JsonConvert.SerializeObject(artista);
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

        public async Task<bool> DeleteArtistaEventoAsync(int idevento, int idartista)
        {
            string requestUri = "api/ArtistasEvento/DeleteArtistaEvento?idevento=" + idevento + "&idartista=" + idartista;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Recuperar el token JWT de la sesión
                string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

                // Añadir el token JWT en el encabezado Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.DeleteAsync(requestUri);

                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteArtistaTempAsync(int idevento, int idartista)
        {
            string requestUri = "api/ArtistasEvento/DeleteArtistaTemp?idevento=" + idevento + "&idartista=" + idartista;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Recuperar el token JWT de la sesión
                string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

                // Añadir el token JWT en el encabezado Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.DeleteAsync(requestUri);

                return response.IsSuccessStatusCode;
            }
        }
        #endregion

        #region Comentarios
        public async Task<bool> AddComentarioAsync(Comentario comentario)
        {
            string requestUri = "api/Comentarios/AddComentario";
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                // Recuperar el token JWT de la sesión
                string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

                // Añadir el token JWT en el encabezado Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string jsonData = JsonConvert.SerializeObject(comentario);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Podrías manejar diferentes tipos de errores aquí y decidir qué hacer con ellos
                    return false;
                }
            }
        }


        public async Task<List<ComentarioDetalles>> GetComentariosEventoAsync(int idevento)
        {
            string request = "/api/Comentarios/GetComentariosEvento?idevento=" + idevento;

            List<ComentarioDetalles> comentarios = await this.CallApiAsync<List<ComentarioDetalles>>(request);

            return comentarios;
        }

        public async Task<bool> DeleteComentarioAsync(int idComentario)
        {
            string requestUri = "api/Comentarios/DeleteComentario?idcoment=" + idComentario;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.urlApiEventos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Recuperar el token JWT de la sesión
                string token = this.httpContextAccessor.HttpContext.Session.GetString("TOKEN");

                // Añadir el token JWT en el encabezado Authorization
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage response = await client.DeleteAsync(requestUri);

                return response.IsSuccessStatusCode;
            }
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

        public async Task<List<EntradaDetalles>> GetAllEntradasUsuarioAsync(int iduser)
        {
            string request = "api/Entradas/VerEntradas/" + iduser;

            List<EntradaDetalles> entradas = await this.CallApiAsync<List<EntradaDetalles>>(request);

            return entradas;
        }
        #endregion
    }
}
