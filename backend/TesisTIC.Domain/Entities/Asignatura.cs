namespace TesisTIC.Domain.Entities
{
    public class Asignatura
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Propuesta> Propuestas { get; set; } = new List<Propuesta>();
    }
}
