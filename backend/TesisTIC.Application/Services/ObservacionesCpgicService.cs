using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services;

/// <summary>
/// HU04 T10: Servicio para observaciones CPGIC
/// Maneja registro, listado y limpieza de observaciones sobre propuestas
/// </summary>
public class ObservacionesCpgicService : IObservacionesCpgicService
{
    private readonly IObservacionesCpgicRepository _repository;
    private readonly IPropuestaRepository _propuestaRepository;
    private readonly IMapper _mapper;

    public ObservacionesCpgicService(
        IObservacionesCpgicRepository repository,
        IPropuestaRepository propuestaRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propuestaRepository = propuestaRepository ?? throw new ArgumentNullException(nameof(propuestaRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Registra una nueva observación sobre una propuesta
    /// Cambiar estado a OBSERVADA automáticamente
    /// </summary>
    public async Task<ObservacionesCpgicDto> CrearObservacionAsync(CreateObservacionesCpgicDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.PropuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");
        if (string.IsNullOrWhiteSpace(dto.Observacion))
            throw new ArgumentException("La observación no puede estar vacía");

        // Verificar que la propuesta existe
        var propuesta = await _propuestaRepository.GetByIdAsync(dto.PropuestaId);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {dto.PropuestaId} no encontrada");

        // Crear la observación
        var observacion = new ObservacionesCpgic
        {
            PropuestaId = dto.PropuestaId,
            Observacion = dto.Observacion.Trim(),
            RealizadoPor = dto.RealizadoPor?.Trim(),
            FechaObservacion = FechaEcuador.Ahora()
        };

        var creada = await _repository.CreateAsync(observacion);

        // Cambiar estado de la propuesta a OBSERVADA
        // Solo si actualmente está en PENDIENTE
        if (propuesta.Estado == "PENDIENTE")
        {
            propuesta.Estado = "OBSERVADA";
            propuesta.FechaActualizacion = FechaEcuador.Ahora();
            await _propuestaRepository.UpdateAsync(propuesta);
        }

        return _mapper.Map<ObservacionesCpgicDto>(creada);
    }

    /// <summary>
    /// Obtiene todas las observaciones de una propuesta
    /// </summary>
    public async Task<IEnumerable<ObservacionesCpgicDto>> ObtenerObservacionesPorPropuestaAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        var observaciones = await _repository.GetByPropuestaIdAsync(propuestaId);
        return _mapper.Map<IEnumerable<ObservacionesCpgicDto>>(observaciones);
    }

    /// <summary>
    /// Obtiene una observación específica
    /// </summary>
    public async Task<ObservacionesCpgicDto?> ObtenerObservacionAsync(int observacionId)
    {
        if (observacionId <= 0)
            throw new ArgumentException("ObservacionId inválido");

        var observacion = await _repository.GetByIdAsync(observacionId);
        return observacion != null ? _mapper.Map<ObservacionesCpgicDto>(observacion) : null;
    }

    /// <summary>
    /// Elimina una observación específica
    /// </summary>
    public async Task<bool> EliminarObservacionAsync(int observacionId)
    {
        if (observacionId <= 0)
            throw new ArgumentException("ObservacionId inválido");

        return await _repository.DeleteAsync(observacionId);
    }

    /// <summary>
    /// Limpia TODAS las observaciones de una propuesta
    /// Se usa cuando se reenvía después de correcciones
    /// También cambia estado OBSERVADA → PENDIENTE
    /// </summary>
    public async Task<bool> LimpiarObservacionesPorPropuestaAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        // Obtener la propuesta
        var propuesta = await _propuestaRepository.GetByIdAsync(propuestaId);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {propuestaId} no encontrada");

        // Eliminar todas las observaciones
        var eliminadas = await _repository.DeleteByPropuestaIdAsync(propuestaId);

        // Cambiar estado OBSERVADA → PENDIENTE
        if (eliminadas > 0 && propuesta.Estado == "OBSERVADA")
        {
            propuesta.Estado = "PENDIENTE";
            propuesta.FechaActualizacion = FechaEcuador.Ahora();
            await _propuestaRepository.UpdateAsync(propuesta);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Verifica si una propuesta tiene observaciones
    /// </summary>
    public async Task<bool> TieneObservacionesAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        return await _repository.TieneObservacionesAsync(propuestaId);
    }

    /// <summary>
    /// Cuenta observaciones de una propuesta
    /// </summary>
    public async Task<int> ContarObservacionesAsync(int propuestaId)
    {
        if (propuestaId <= 0)
            throw new ArgumentException("PropuestaId inválido");

        return await _repository.CountByPropuestaIdAsync(propuestaId);
    }
}
