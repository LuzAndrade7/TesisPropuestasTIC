namespace TesisTIC.Application.Services;

/// <summary>
/// Provee fechas con calendario de Ecuador para registros visibles en el sistema.
/// </summary>
public static class FechaEcuador
{
    /// <summary>
    /// Obtiene la fecha y hora actual de Ecuador manteniendo Kind UTC para compatibilidad con Npgsql.
    /// </summary>
    public static DateTime Ahora()
    {
        var zona = ObtenerZonaEcuador();
        var fechaEcuador = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona);
        return DateTime.SpecifyKind(fechaEcuador, DateTimeKind.Utc);
    }

    /// <summary>
    /// Resuelve la zona horaria de Ecuador en Windows y Linux.
    /// </summary>
    private static TimeZoneInfo ObtenerZonaEcuador()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");
        }
    }
}
