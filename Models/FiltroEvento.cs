namespace MvcCoreProyectoSejo.Models
{
    public class FiltroEvento
    {
        public string? Nombre { get; set; }
        public DateTime? FechaInicio { get; set; }
        public string? Provincia { get; set; }
        public string? Tipo { get; set; }
        public decimal? PrecioMayorQue { get; set; }
        public decimal? PrecioMenorQue { get; set; }

        public bool TieneFiltros()
        {
            return !string.IsNullOrEmpty(Nombre) || FechaInicio.HasValue || !string.IsNullOrEmpty(Provincia) || !string.IsNullOrEmpty(Tipo) || PrecioMayorQue.HasValue || PrecioMenorQue.HasValue;
        }
    }
}
