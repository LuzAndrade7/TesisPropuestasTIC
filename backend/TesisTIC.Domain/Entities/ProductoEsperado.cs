namespace TesisTIC.Domain.Entities;

/// <summary>
/// Entidad que representa un producto esperado de un componente
/// Mapea la tabla public.productos_esperados
/// </summary>
public class ProductoEsperado
{
    public int Id { get; set; }

    /// <summary>
    /// ID del componente al que pertenece este producto esperado
    /// </summary>
    public int ComponenteId { get; set; }

    /// <summary>
    /// Descripción del producto esperado
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    // Relaciones
    /// <summary>
    /// Componente al que pertenece este producto esperado
    /// </summary>
    public virtual Componente Componente { get; set; } = null!;
}
