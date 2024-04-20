using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace ApiCoreProyectoEventos.Filters
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] rolesPermitidos;

        public AuthorizeRolesAttribute(params string[] roles)
        {
            rolesPermitidos = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                RouteValueDictionary routeLogin =
                    new RouteValueDictionary(new
                    {
                        controller = "Usuarios",
                        action = "Login"
                    });
                context.Result =
                    new RedirectToRouteResult(routeLogin);
            }
            else
            {
                //if (user.IsInRole("4")) // Admin role
                //{
                //    // Redirecciona al panel de admin si el usuario es admin
                //    context.Result = GetRoute("Admin", "Dashboard");
                //    return;
                //}

                // Comprueba si el usuario tiene alguno de los roles permitidos
                bool authorized = false;
                foreach (var role in rolesPermitidos)
                {
                    if (user.IsInRole(role))
                    {
                        authorized = true;
                        break;
                    }
                }

                if (!authorized)
                {
                    context.Result = GetRoute("Usuarios", "ErrorAcceso");
                }
            }
        }

        private RedirectToRouteResult GetRoute(string controller, string action)
        {
            return new RedirectToRouteResult(
                new { controller = controller, action = action });
        }
    }
}
