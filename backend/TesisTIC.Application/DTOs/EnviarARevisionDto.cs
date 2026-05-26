namespace TesisTIC.Application.DTOs;

/// <summary>
/// DTO para enviar una propuesta a revisión (HU03 - T07)
/// Valida que la propuesta tenga toda la información requerida antes de enviarla
/// 
/// Validaciones:
/// - Nombre proyecto: 10-250 caracteres
/// - Número participantes: 1-5
/// - Descripción: mínimo 20 caracteres
/// - Objetivo: no vacío
/// - Alcance: no vacío
/// - Mínimo 1 asignatura asignada
/// - Estado actual debe ser BORRADOR
/// </summary>
public class EnviarARevisionDto
{
    /// <summary>
    /// Campo de confirmación (opcional, puede ser vacío)
    /// El servidor valida todos los campos de la propuesta
    /// </summary>
    public string? Razon { get; set; }
}
