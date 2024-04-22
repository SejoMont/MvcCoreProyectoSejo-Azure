using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcCoreProyectoSejo.Extensions;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;
using MvcCoreProyectoSejo.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using ApiCoreProyectoEventos.Filters;
using System.Security.Principal;

namespace MvcCoreProyectoSejo.Controllers
{
    public class UsuariosController : Controller
    {

        private ServiceEventos service;


        private HelperMails helperMails;
        private HelperPathProvider helperPathProvider;

        public UsuariosController(HelperMails helperMails, HelperPathProvider helperPathProvider, ServiceEventos service)
        {

            this.helperMails = helperMails;
            this.helperPathProvider = helperPathProvider;
            this.service = service;
        }

        public async Task<IActionResult> Details(int iduser)
        {
            UsuarioDetalles usuarioDetalles = await this.service.GetUsuarioDetallesAsync(iduser);
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
                    eventosAsociados = await this.service.GetAllEventosArtistaAsync(iduser);
                    break;
                case 3: // Recinto
                        // Asume la existencia de un método que obtiene eventos asociados a un recinto
                    eventosAsociados = await this.service.GetEventosPorRecintoAsync(iduser);
                    break;
            }

            DateTime fechaHoy = DateTime.Today;

            List<EventoDetalles> eventosPasados = eventosAsociados.Where(e => e.Fecha < fechaHoy).OrderBy(e => e.Fecha).ToList();
            List<EventoDetalles> eventosProximos = eventosAsociados.Where(e => e.Fecha >= fechaHoy).OrderBy(e => e.Fecha).ToList();

            ViewData["EventosPasados"] = eventosPasados;
            ViewData["EventosProximos"] = eventosProximos;

            return View(usuarioDetalles);
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            string loginToken = await this.service.Login(login);
            if (loginToken != null)
            {
                // Validar el token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(loginToken) as JwtSecurityToken;

                if (token != null)
                {
                    // Extraer la información del usuario del token
                    var userDataClaim = token.Claims.First(claim => claim.Type == "UserData").Value;
                    var userData = JsonConvert.DeserializeObject<Usuario>(userDataClaim);

                    // Configuración de la identidad y claims
                    ClaimsIdentity identity = new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, ClaimTypes.Role);

                    // Agregar claims básicos
                    identity.AddClaim(new Claim(ClaimTypes.Name, userData.NombreUsuario));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userData.UsuarioID.ToString()));

                    // Puedes agregar más claims según necesites
                    identity.AddClaim(new Claim("Email", userData.Correo));
                    identity.AddClaim(new Claim(ClaimTypes.Role, (userData.RolID).ToString()));

                    //Guardar el token en la sesión
                    HttpContext.Session.SetString("TOKEN", loginToken);

                    // Crear el principal y autenticar al usuario
                    ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

                    // Establecer la autenticación de cookies con las claims y expiración
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true, // para que la cookie persista entre sesiones del navegador
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // establece la misma expiración que el token JWT
                        });

                    // Redireccionar a la vista principal del usuario
                    return RedirectToAction("Index", "Eventos");
                }
            }

            ViewData["Mensaje"] = "Credenciales incorrectas. Por favor, inténtalo de nuevo.";
            return View();
        }

        [AuthorizeRoles]
        public IActionResult LogFalso()
        {
            return RedirectToAction("Index", "Eventos");
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Registro registro)
        {
            if (!string.IsNullOrEmpty(registro.Correo) && await this.service.EmailExists(registro.Correo))
            {
                ViewData["Mensaje"] = "El correo electrónico ya está en uso. Por favor, utiliza otro.";
                return View();
            }

            if (registro.Password != registro.ConfirmPassword)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden. Por favor, inténtalo de nuevo.";
                return View();
            }

            // Llamar al método RegisterUserAsync con la contraseña tal como está
            Usuario user = await this.service.RegisterUserAsync(registro);

            // Verificar si el usuario se creó correctamente
            if (user != null)
            {
                HttpContext.Session.SetObject("CurrentUser", user);

                //string serverUrl = this.helperPathProvider.MapUrlServerPath();
                ////https://localhost:8555/Usuarios/ActivateUser/TOKEN??? 
                //serverUrl = serverUrl + "/Usuarios/ActivateUser/?token=" + user.TokenMail;
                //string mensaje = "<h3>Usuario registrado</h3>";
                //mensaje += "<p>Debe activar su cuenta con nosotros pulsando el siguiente enlace</p>";
                //mensaje += "<p><a href='" + serverUrl + "'>" + serverUrl + "</a></p>";
                //mensaje += "<p>Muchas gracias</p>";
                //await this.helperMails.SendMailAsync(registro.Correo, "Registro Usuario", mensaje);

                return RedirectToAction("Login");
            }
            else
            {
                // Manejar la situación en la que el usuario no se creó correctamente
                ViewData["Mensaje"] = "No se pudo crear el usuario correctamente. Por favor, inténtalo de nuevo.";
                return View();
            }
        }


        //public async Task<IActionResult> ActivateUser(string token)
        //{
        //    await this.repo.ActivateUserAsync(token);
        //    ViewData["MENSAJE"] = "Cuenta activada correctamente";
        //    return RedirectToAction("Index", "Eventos");
        //}

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("CurrentUser");
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        //public async Task<IActionResult> Edit(int id)
        //{
        //    UsuarioDetalles usuarioDetalles = await this.repo.GetUsuarioDetalles(id);
        //    if (usuarioDetalles == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(usuarioDetalles);
        //}

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
