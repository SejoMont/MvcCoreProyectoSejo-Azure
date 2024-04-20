namespace MvcCoreProyectoSejo.Models
{
    public class Registro
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Rol { get; set; }
    }
}
