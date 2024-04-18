#region VIEWS Y PROCEDURES
//ALTER VIEW VISTA_DETALLE_USUARIO AS
//SELECT
//    U.UsuarioID,
//    U.NombreUsuario,
//    U.FotoPerfil,
//    U.Correo,
//    U.Telefono,
//    U.ProvinciaID,
//    P.NombreProvincia,
//    U.Descripcion,
//    U.Activo,
//    R.NombreRol,
//    R.RolID
//FROM Usuarios U
//INNER JOIN Roles R ON U.RolID = R.RolID
//INNER JOIN Provincias P ON U.ProvinciaID = P.ProvinciaID;

//create view vista_detalle_artista as
//select
//    u.usuarioid,
//u.nombreusuario,
//    u.fotoperfil,
//    u.provinciaid,
//    p.nombreprovincia,
//    u.descripcion,
//    r.nombrerol,
//    r.RolID,
//    ae.eventoid
//from usuarios u
//inner join artistasevento ae on u.usuarioid = ae.artistaid
//inner join roles r on u.rolid = r.rolid
//inner join provincias p on u.provinciaid = p.provinciaid;  
#endregion

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProyectoSejo.Helpers;
using MvcCoreProyectoSejo.Models;

namespace MvcCoreProyectoSejo.Repository
{
    public class UsuariosRepository
    {
        private EventosContext context;

        public UsuariosRepository(EventosContext context)
        {
            this.context = context;
        }
        public async Task<List<ArtistaDetalles>> GetAllArtistasEventoAsync(int idevento)
        {
            var artistas = await this.context.ArtistasDetalles
                .Where(a => a.EventoID == idevento)
                .ToListAsync();

            return artistas;
        }


        public async Task<List<UsuarioDetalles>> GetAllArtistas()
        {
            var artistas = await this.context.UsuariosDetalles
                .Where(r => r.RolID == 2)
                .ToListAsync();

            return artistas;
        }

        public async Task UpdateUserAsync(Usuario usuario)
        {
            this.context.Usuarios.Update(usuario);
            await this.context.SaveChangesAsync();
        }


        public async Task<UsuarioDetalles> GetUsuarioDetalles(int iduser)
        {
            return await this.context.UsuariosDetalles.FirstOrDefaultAsync(z => z.UsuarioID == iduser);
        }

        //---------------------- Registro / Login ----------------------//
        public bool EmailExists(string email)
        {
            var consulta = from u in context.Usuarios
                           where u.Correo == email
                           select u;

            return consulta.Any();
        }

        public async Task<Usuario> RegisterUserAsync(string nombre, string email
             , string password, int rol)
        {
            Usuario user = new Usuario();
            user.NombreUsuario = nombre;
            user.Correo = email;
            user.RolID = rol;
            user.ProvinciaID = 1;
            user.Telefono = "";
            user.FotoPerfil = "default-user.png";
            user.Descripcion = "";
            user.Activo = false;
            //CADA USUARIO TENDRA UN SALT DISTINTO 
            user.Salt = HelperTools.GenerateSalt();
            //GUARDAMOS EL PASSWORD EN BYTE[] 
            user.Password =
                HelperCryptography.EncryptPassword(password, user.Salt);
            user.Activo = false;
            user.TokenMail = HelperTools.GenerateTokenMail();

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        public async Task ActivateUserAsync(string token)
        {
            // Buscamos el usuario por su token
            Usuario user = await this.context.Usuarios.FirstOrDefaultAsync(x => x.TokenMail == token);

            user.Activo = true;

            user.TokenMail = "";

            await this.context.SaveChangesAsync();
        }

        public async Task<bool> LogInUserAsync(string correo, string password)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario != null)
            {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = usuario.Password;
                bool response = HelperTools.CompareArrays(temp, passUser);

                return response;
            }
            else
            {
                return false;
            }
        }
        public Usuario GetUser(string correo)
        {
            var usuario = (from u in context.Usuarios
                           where u.Correo == correo
                           select u).FirstOrDefault();

            return usuario;
        }
    }
}