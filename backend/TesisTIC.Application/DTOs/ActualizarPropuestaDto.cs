namespace TesisTIC.Application.DTOs
{
    public class ActualizarPropuestaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Objetivo { get; set; }
        public string Alcance { get; set; }
        public string ComponentesActividadesProductos { get; set; }
        public int? LineaInvestigacionId { get; set; }
        public string? Observaciones { get; set; }
        public int NumeroParticipantes { get; set; }
        public string Departamento { get; set; }
        public string Facultad { get; set; }
        public List<int> AsignaturasIds { get; set; } = new List<int>();
    }
}
