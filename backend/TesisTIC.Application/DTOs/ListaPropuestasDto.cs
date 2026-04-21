namespace TesisTIC.Application.DTOs
{
    public class ListaPropuestasDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public string Proyecto { get; set; }
        public bool TieneObservaciones { get; set; }
        public int ModulosSinEstudiante { get; set; }
    }
}
