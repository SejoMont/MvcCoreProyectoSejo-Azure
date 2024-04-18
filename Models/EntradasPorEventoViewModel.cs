namespace MvcCoreProyectoSejo.Models
{
    public class EntradasPorEventoViewModel
    {
        public int EventoID { get; set; }
        public int NumeroDeEntradas { get; set; }
        public List<EntradaDetalles> Entradas { get; set; }

        // Propiedades adicionales para los detalles del evento
        public string NombreEvento { get; set; }
        public string Provincia { get; set; }
        public string TipoEvento { get; set; }
        public int Precio { get; set; }
        public string Imagen { get; set; }
    }
}
