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
                string request = "api/UsuariosApi/Login";
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
            string request = "api/UsuariosApi/GetUser/"+correo;

            Usuario user = await this.CallApiSync<Usuario>(request);

            return user;
        }
        #endregion

        #region Eventos
        public async Task<List<EventoDetalles>> GetEventosAsync()
        {
            string request = "api/Eventos/GetEventos";

            List<EventoDetalles> eventos = await this.CallApiSync<List<EventoDetalles>>(request);

            return eventos;
        }
        #endregion
    }
}
