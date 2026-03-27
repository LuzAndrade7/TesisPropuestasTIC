namespace TesisTIC.Domain.Entities
{
    public class Estado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<Propuesta> Propuestas { get; set; } = new List<Propuesta>();
    }
}
