namespace TesisTIC.Domain.Entities
{
    public class ObservacionPropuesta
    {
        public int Id { get; set; }
        public int PropuestaId { get; set; }
        public string Autor { get; set; }
        public string Texto { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Propuesta Propuesta { get; set; }
    }
}
