namespace TesisTIC.Domain.Entities
{
    public class RechazoPropuesta
    {
        public int Id { get; set; }
        public int PropuestaId { get; set; }
        public string Autor { get; set; }
        public DateTime Fecha { get; set; }
        public string Recomendacion { get; set; }

        public virtual Propuesta Propuesta { get; set; }
        public virtual ICollection<RechazoRazon> Razones { get; set; } = new List<RechazoRazon>();
    }
}
