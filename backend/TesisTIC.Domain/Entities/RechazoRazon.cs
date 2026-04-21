namespace TesisTIC.Domain.Entities
{
    public class RechazoRazon
    {
        public int Id { get; set; }
        public int RechazoPropuestaId { get; set; }
        public string Razon { get; set; }

        public virtual RechazoPropuesta RechazoPropuesta { get; set; }
    }
}
