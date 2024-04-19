using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcCoreProyectoSejo.Extensions;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Repository;
using MvcCoreProyectoSejo.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace MvcCoreProyectoSejo.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuariosRepository repo;
        private ServiceEventos service;
        private EventosRepository eventosRepo;

        private HelperMails helperMails;
        private HelperPathProvider helperPathProvider;

        public UsuariosController(UsuariosRepository repo, EventosRepository eventosRepo, HelperMails helperMails, HelperPathProvider helperPathProvider, ServiceEventos service)
        {
            this.repo = repo;
            this.eventosRepo = eventosRepo;
            this.helperMails = helperMails;
            this.helperPathProvider = helperPathProvider;
            this.service = service;
        }

        public async Task<IActionResult> Details(int iduser)
        {
            UsuarioDetalles usuarioDetalles = await this.repo.GetUsuarioDetalles(iduser);
            // Asume que el rol del usuario está almacenado en el modelo UsuarioDetalles
            int rolId = usuarioDetalles.RolID;

            List<EventoDetalles> eventosAsociados = new List<EventoDetalles>();

            switch (rolId)
            {
                case 1: // Usuario Corriente
                        // Asume la existencia de un método que obtiene eventos por asistencia para un usuario
                        //eventosAsociados = await this.eventosRepo.GetEventosPorAsistenciaUsuarioAsync(iduser);
                    break;
                case 2: // Artista
                    eventosAsociados = await this.eventosRepo.GetAllEventosArtistaAsync(iduser);
                    break;
                case 3: // Recinto
                        // Asume la existencia de un método que obtiene eventos asociados a un recinto
                    eventosAsociados = await this.eventosRepo.GetEventosPorRecintoAsync(iduser);
                    break;
            }

            DateTime fechaHoy = DateTime.Today;

            List<EventoDetalles> eventosPasados = eventosAsociados.Where(e => e.Fecha < fechaHoy).OrderBy(e => e.Fecha).ToList();
            List<EventoDetalles> eventosProximos = eventosAsociados.Where(e => e.Fecha >= fechaHoy).OrderBy(e => e.Fecha).ToList();

            ViewData["EventosPasados"] = eventosPasados;
            ViewData["EventosProximos"] = eventosProximos;

            return View(usuarioDetalles);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            string login = await this.service.Login(correo, password);
            if (login != null)
            {
                // Validar el token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(login) as JwtSecurityToken;

                if (token != null)
                {
                    // Extraer la información del usuario del token
                    var userDataClaim = token.Claims.First(claim => claim.Type == "UserData").Value;
                    var userData = JsonConvert.DeserializeObject<Usuario>(userDataClaim);

                    // Establecer el usuario en la sesión
                    HttpContext.Session.SetObject("CurrentUser", userData);

                    return RedirectToAction("Index", "Eventos", new { iduser = userData.UsuarioID });
                }
            }

            ViewData["Mensaje"] = "Credenciales incorrectas. Por favor, inténtalo de nuevo.";
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string nombre, string password, string confirmPassword, string correo)
        {
            if (!string.IsNullOrEmpty(correo) && repo.EmailExists(correo))
            {
                ViewData["Mensaje"] = "El correo electrónico ya está en uso. Por favor, utiliza otro.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden. Por favor, inténtalo de nuevo.";
                return View();
            }

            // Llamar al método RegisterUserAsync con la contraseña tal como está
            Usuario user = await repo.RegisterUserAsync(nombre, correo, password, 1);

            // Verificar si el usuario se creó correctamente
            if (user != null)
            {
                HttpContext.Session.SetObject("CurrentUser", user);

                string serverUrl = this.helperPathProvider.MapUrlServerPath();
                //https://localhost:8555/Usuarios/ActivateUser/TOKEN??? 
                serverUrl = serverUrl + "/Usuarios/ActivateUser/?token=" + user.TokenMail;
                string mensaje = "<h3>Usuario registrado</h3>";
                mensaje += "<p>Debe activar su cuenta con nosotros pulsando el siguiente enlace</p>";
                mensaje += "<p><a href='" + serverUrl + "'>" + serverUrl + "</a></p>";
                mensaje += "<p>Muchas gracias</p>";
                await this.helperMails.SendMailAsync(correo, "Registro Usuario", mensaje);

                return RedirectToAction("Login");
            }
            else
            {
                // Manejar la situación en la que el usuario no se creó correctamente
                ViewData["Mensaje"] = "No se pudo crear el usuario correctamente. Por favor, inténtalo de nuevo.";
                return View();
            }
        }


        public async Task<IActionResult> ActivateUser(string token)
        {
            await this.repo.ActivateUserAsync(token);
            ViewData["MENSAJE"] = "Cuenta activada correctamente";
            return RedirectToAction("Index", "Eventos");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentUser");
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Edit(int id)
        {
            UsuarioDetalles usuarioDetalles = await this.repo.GetUsuarioDetalles(id);
            if (usuarioDetalles == null)
            {
                return NotFound();
            }
            return View(usuarioDetalles);
        }
    }
}
