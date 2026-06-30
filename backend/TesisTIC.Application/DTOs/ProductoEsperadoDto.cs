namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para mostrar información de un producto esperado
/// </summary>
public class ProductoEsperadoDto
{
    public int Id { get; set; }
    public int ComponenteId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para crear un producto esperado
/// </summary>
public class CreateProductoEsperadoDto
{
    public int ComponenteId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para actualizar un producto esperado
/// </summary>
public class UpdateProductoEsperadoDto
{
    public string? Descripcion { get; set; }
}
