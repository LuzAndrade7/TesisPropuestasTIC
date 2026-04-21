namespace TesisTIC.Application.DTOs
{
    public class RechazoRazonDto
    {
        public int Id { get; set; }
        public string Razon { get; set; }
    }

    public class RechazoPropuestaDto
    {
        public int Id { get; set; }
        public string Autor { get; set; }
        public DateTime Fecha { get; set; }
        public List<string> Razones { get; set; } = new();
        public string Recomendacion { get; set; }
    }
}
