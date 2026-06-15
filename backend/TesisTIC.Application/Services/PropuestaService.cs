using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services;

/// <summary>
/// Servicio para operaciones de propuestas
/// Maneja mapeo de DTOs, validaciones y lÃ³gica de negocio
/// </summary>
public class PropuestaService : IPropuestaService
{
    private readonly IPropuestaRepository _repository;
    private readonly IAsignaturaRepository _asignaturaRepository;
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IMapper _mapper;

    public PropuestaService(
        IPropuestaRepository repository,
        IAsignaturaRepository asignaturaRepository,
        IEstudianteRepository estudianteRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _asignaturaRepository = asignaturaRepository ?? throw new ArgumentNullException(nameof(asignaturaRepository));
        _estudianteRepository = estudianteRepository ?? throw new ArgumentNullException(nameof(estudianteRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // ===== LECTURA =====

    /// <summary>
    /// Obtiene todas las propuestas
    /// </summary>
    public async Task<IEnumerable<PropuestaDto>> GetAllAsync()
    {
        var propuestas = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PropuestaDto>>(propuestas);
    }

    /// <summary>
    /// Obtiene una propuesta por ID con todas sus relaciones
    /// </summary>
    public async Task<PropuestaDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));

        var propuesta = await _repository.GetPropuestaFullAsync(id);
        return propuesta != null ? _mapper.Map<PropuestaDto>(propuesta) : null;
    }

    /// <summary>
    /// Obtiene propuestas filtradas por estado
    /// </summary>
    public async Task<IEnumerable<PropuestaDto>> GetPorEstadoAsync(string estado)
    {
        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("Estado no puede estar vacÃ­o", nameof(estado));

        // Validar estado
        var estadosValidos = new[] { "BORRADOR", "PENDIENTE", "OBSERVADA", "APROBADA", "RECHAZADA" };
        if (!estadosValidos.Contains(estado))
            throw new ArgumentException($"Estado invÃ¡lido: {estado}");

        var propuestas = await _repository.GetPropuestasPorEstadoAsync(estado);
        return _mapper.Map<IEnumerable<PropuestaDto>>(propuestas);
    }

    /// <summary>
    /// Obtiene propuestas de un profesor especÃ­fico
    /// </summary>
    public async Task<IEnumerable<PropuestaDto>> GetPorProfesorAsync(int profesorId)
    {
        if (profesorId <= 0)
            throw new ArgumentException("ID de profesor invÃ¡lido", nameof(profesorId));

        var propuestas = await _repository.GetPropuestasPorProfesorAsync(profesorId);
        return _mapper.Map<IEnumerable<PropuestaDto>>(propuestas);
    }

    // ===== ESCRITURA =====

    /// <summary>
    /// Crea una nueva propuesta en estado BORRADOR (SIN validaciones - borrador permisivo)
    /// </summary>
    public async Task<PropuestaDto> CreateAsync(CreatePropuestaDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var ahora = FechaEcuador.Ahora();

        var propuesta = new Propuesta
        {
            NombreProyecto = dto.NombreProyecto ?? string.Empty,
            NumeroParticipantes = dto.NumeroParticipantes,
            ProfesorId = dto.ProfesorId,
            Descripcion = dto.Descripcion ?? string.Empty,
            Objetivo = dto.Objetivo ?? string.Empty,
            Alcance = dto.Alcance ?? string.Empty,
            Estado = "BORRADOR",
            FechaCreacion = ahora,
            FechaActualizacion = ahora
        };

        // Agregar asignaturas si se proporcionan
        if (dto.AsignaturaIds != null && dto.AsignaturaIds.Any())
        {
            foreach (var asignaturaId in dto.AsignaturaIds)
            {
                propuesta.PropuestasAsignaturas.Add(new PropuestaAsignatura
                {
                    AsignaturaId = asignaturaId
                });
            }
        }

        await SincronizarComponentesAsync(propuesta, dto.Componentes);

        var creada = await _repository.CreateAsync(propuesta);
        return _mapper.Map<PropuestaDto>(creada);
    }

    /// <summary>
    /// HU05 T14: Actualiza una propuesta existente
    /// Solo permite editar si estado es BORRADOR u OBSERVADA
    /// ActualizaciÃ³n parcial: solo campos no null se actualizan
    /// </summary>
    public async Task<PropuestaDto> UpdateAsync(int id, UpdatePropuestaDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var propuesta = await _repository.GetPropuestaFullAsync(id);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {id} no encontrada");

        // HU05 T14: Validar que solo se edita en estado BORRADOR u OBSERVADA
        if (propuesta.Estado != "BORRADOR" && propuesta.Estado != "OBSERVADA")
            throw new InvalidOperationException(
                $"No se puede editar una propuesta en estado '{propuesta.Estado}'. " +
                $"Solo se pueden editar propuestas en estado BORRADOR u OBSERVADA.");

        // Solo actualizar campos que no sean null
        if (!string.IsNullOrWhiteSpace(dto.NombreProyecto))
            propuesta.NombreProyecto = dto.NombreProyecto.Trim();
        if (dto.NumeroParticipantes.HasValue && dto.NumeroParticipantes > 0)
            propuesta.NumeroParticipantes = dto.NumeroParticipantes.Value;
        if (!string.IsNullOrWhiteSpace(dto.Descripcion))
            propuesta.Descripcion = dto.Descripcion.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Objetivo))
            propuesta.Objetivo = dto.Objetivo.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Alcance))
            propuesta.Alcance = dto.Alcance.Trim();

        // Actualizar asignaturas si se proporcionan
        if (dto.AsignaturaIds != null)
        {
            // Validar que hay al menos una asignatura
            if (!dto.AsignaturaIds.Any())
                throw new ArgumentException("Debe asignar al menos una asignatura a la propuesta");

            // Eliminar asignaturas que ya no estÃ¡n en la lista
            propuesta.PropuestasAsignaturas.Clear();

            // Agregar las nuevas asignaturas
            foreach (var asignaturaId in dto.AsignaturaIds)
            {
                propuesta.PropuestasAsignaturas.Add(new PropuestaAsignatura
                {
                    PropuestaId = propuesta.Id,
                    AsignaturaId = asignaturaId
                });
            }
        }

        if (dto.Componentes != null)
        {
            await SincronizarComponentesAsync(propuesta, dto.Componentes);
        }

        // Asegurar que todos los DateTime sean UTC antes de guardar (requerido para PostgreSQL)
        if (propuesta.FechaCreacion.Kind == DateTimeKind.Unspecified)
            propuesta.FechaCreacion = DateTime.SpecifyKind(propuesta.FechaCreacion, DateTimeKind.Utc);

        if (propuesta.FechaEnvioRevision.HasValue && propuesta.FechaEnvioRevision.Value.Kind == DateTimeKind.Unspecified)
            propuesta.FechaEnvioRevision = DateTime.SpecifyKind(propuesta.FechaEnvioRevision.Value, DateTimeKind.Utc);

        propuesta.FechaActualizacion = FechaEcuador.Ahora();

        var actualizada = await _repository.UpdateAsync(propuesta);
        return _mapper.Map<PropuestaDto>(actualizada);
    }

    /// <summary>
    /// Reemplaza los componentes de una propuesta con los datos enviados por el formulario.
    /// </summary>
    private async Task SincronizarComponentesAsync(Propuesta propuesta, List<CreateComponenteConDetalleDto>? componentesDto)
    {
        propuesta.Componentes.Clear();

        if (componentesDto == null || !componentesDto.Any())
        {
            return;
        }

        var orden = 1;
        foreach (var componenteDto in componentesDto)
        {
            var nombre = componenteDto.Nombre?.Trim() ?? string.Empty;
            var descripcion = componenteDto.Descripcion?.Trim() ?? string.Empty;
            var actividadesValidas = componenteDto.Actividades
                .Where(a => !string.IsNullOrWhiteSpace(a.Descripcion) || a.Horas > 0)
                .ToList();
            var productosValidos = componenteDto.ProductosEsperados
                .Where(p => !string.IsNullOrWhiteSpace(p.Descripcion))
                .ToList();

            if (string.IsNullOrWhiteSpace(nombre) &&
                string.IsNullOrWhiteSpace(descripcion) &&
                !actividadesValidas.Any() &&
                !productosValidos.Any())
            {
                continue;
            }

            var componente = new Componente
            {
                PropuestaId = propuesta.Id,
                Nombre = nombre,
                Descripcion = descripcion,
                Orden = componenteDto.Orden > 0 ? componenteDto.Orden : orden
            };

            var nombreEstudiante = componenteDto.NombresEstudiante?.Trim();
            if (!string.IsNullOrWhiteSpace(nombreEstudiante))
            {
                var estudiante = await _estudianteRepository.GetByNombreEstudianteAsync(nombreEstudiante);
                if (estudiante == null)
                {
                    estudiante = await _estudianteRepository.CreateAsync(new Estudiante
                    {
                        NombresEstudiante = nombreEstudiante
                    });
                }

                componente.EstudianteId = estudiante.Id;
            }

            var numeroActividad = 1;
            foreach (var actividadDto in actividadesValidas)
            {
                componente.Actividades.Add(new Actividad
                {
                    Numero = actividadDto.Numero > 0 ? actividadDto.Numero : numeroActividad,
                    Descripcion = actividadDto.Descripcion?.Trim() ?? string.Empty,
                    Horas = actividadDto.Horas
                });
                numeroActividad++;
            }

            foreach (var productoDto in productosValidos)
            {
                componente.ProductosEsperados.Add(new ProductoEsperado
                {
                    Descripcion = productoDto.Descripcion.Trim()
                });
            }

            propuesta.Componentes.Add(componente);
            orden++;
        }
    }

    /// <summary>
    /// HU08 T26: Elimina una propuesta (solo si estÃ¡ en estado BORRADOR)
    /// EliminaciÃ³n FÃSICA: Las cascadas en FK garantizan que relaciones se eliminen
    /// 
    /// ValidaciÃ³n:
    /// - Solo propuestas BORRADOR se pueden eliminar
    /// - PENDIENTE, OBSERVADA, APROBADA, RECHAZADA NO se pueden eliminar
    /// 
    /// Integridad:
    /// - ON DELETE CASCADE elimina automÃ¡ticamente:
    ///   * propuesta_asignaturas
    ///   * propuesta_estudiantes
    ///   * observaciones_cpgic
    ///   * historial_estados
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));

        // Obtener propuesta para validar estado
        var propuesta = await _repository.GetByIdAsync(id);
        if (propuesta == null)
            throw new KeyNotFoundException($"Propuesta con ID {id} no encontrada");

        // HU08: VALIDACIÃ“N - Solo se pueden eliminar propuestas BORRADOR
        if (propuesta.Estado != "BORRADOR")
            throw new InvalidOperationException(
                $"No se puede eliminar propuesta en estado {propuesta.Estado}. " +
                "Solo se pueden eliminar propuestas en estado BORRADOR.");

        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Cambia el estado de una propuesta
    /// </summary>
    public async Task<PropuestaDto> CambiarEstadoAsync(int id, UpdateEstadoPropuestaDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));
        if (dto == null || string.IsNullOrWhiteSpace(dto.Estado))
            throw new ArgumentException("Estado no puede estar vacÃ­o");

        var propuesta = await _repository.CambiarEstadoAsync(id, dto.Estado);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {id} no encontrada");

        return _mapper.Map<PropuestaDto>(propuesta);
    }

    /// <summary>
    /// HU03 T07: EnvÃ­a una propuesta a revisiÃ³n (cambia estado BORRADOR -> PENDIENTE)
    /// Realiza validaciones completas antes de permitir el envÃ­o:
    /// - Nombre proyecto: 10-250 caracteres
    /// - NÃºmero participantes: 1-5
    /// - DescripciÃ³n: mÃ­nimo 20 caracteres
    /// - Objetivo: no vacÃ­o
    /// - Alcance: no vacÃ­o
    /// - MÃ­nimo 1 asignatura asignada
    /// - Estado actual debe ser BORRADOR
    /// </summary>
    public async Task<PropuestaDto> EnviarARevisionAsync(int id, EnviarARevisionDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));

        // Obtener propuesta con todas sus relaciones
        var propuesta = await _repository.GetPropuestaFullAsync(id);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {id} no encontrada");

        // ValidaciÃ³n 1: Estado actual debe ser BORRADOR
        if (propuesta.Estado != "BORRADOR")
            throw new InvalidOperationException($"La propuesta debe estar en estado BORRADOR para enviarla a revisiÃ³n. Estado actual: {propuesta.Estado}");

        // ValidaciÃ³n 2: Nombre proyecto
        if (string.IsNullOrWhiteSpace(propuesta.NombreProyecto))
            throw new ArgumentException("El nombre del proyecto es requerido");
        if (propuesta.NombreProyecto.Length < 10)
            throw new ArgumentException("El nombre del proyecto debe tener al menos 10 caracteres");
        if (propuesta.NombreProyecto.Length > 500)
            throw new ArgumentException("El nombre del proyecto no puede exceder 500 caracteres");

        // ValidaciÃ³n 3: NÃºmero de participantes
        if (propuesta.NumeroParticipantes <= 0 || propuesta.NumeroParticipantes > 5)
            throw new ArgumentException("El nÃºmero de participantes debe estar entre 1 y 5");

        // ValidaciÃ³n 4: DescripciÃ³n
        if (string.IsNullOrWhiteSpace(propuesta.Descripcion))
            throw new ArgumentException("La descripciÃ³n de la propuesta es requerida");
        if (propuesta.Descripcion.Length < 20)
            throw new ArgumentException("La descripciÃ³n debe tener al menos 20 caracteres");

        // ValidaciÃ³n 5: Objetivo
        if (string.IsNullOrWhiteSpace(propuesta.Objetivo))
            throw new ArgumentException("El objetivo de la propuesta es requerido");

        // ValidaciÃ³n 6: Alcance
        if (string.IsNullOrWhiteSpace(propuesta.Alcance))
            throw new ArgumentException("El alcance de la propuesta es requerido");

        // ValidaciÃ³n 7: MÃ­nimo una asignatura
        if (propuesta.PropuestasAsignaturas == null || !propuesta.PropuestasAsignaturas.Any())
            throw new ArgumentException("Debe asignar al menos una asignatura a la propuesta");

        // Si todas las validaciones pasan, cambiar estado a PENDIENTE
        propuesta.Estado = "PENDIENTE";
        // Asegurar que los DateTime sean UTC
        propuesta.FechaEnvioRevision = FechaEcuador.Ahora();
        propuesta.FechaActualizacion = FechaEcuador.Ahora();
        propuesta.FechaCreacion = DateTime.SpecifyKind(propuesta.FechaCreacion, DateTimeKind.Utc);

        // Actualizar en base de datos
        var actualizada = await _repository.UpdateAsync(propuesta);
        return _mapper.Map<PropuestaDto>(actualizada);
    }

    /// <summary>
    /// HU04 T12: ReenvÃ­a una propuesta despuÃ©s de correcciones
    /// Limpia TODAS las observaciones y cambia estado OBSERVADA â†’ PENDIENTE
    /// Solo se puede reenviar si estÃ¡ en estado OBSERVADA
    /// </summary>
    public async Task<PropuestaDto> ReenviarDespuesDeObservacionesAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID invÃ¡lido", nameof(id));

        var propuesta = await _repository.GetPropuestaFullAsync(id);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {id} no encontrada");

        // Validar que estÃ¡ en estado OBSERVADA
        if (propuesta.Estado != "OBSERVADA")
            throw new InvalidOperationException($"La propuesta debe estar en estado OBSERVADA para reenviarla. Estado actual: {propuesta.Estado}");

        // Cambiar estado OBSERVADA â†’ PENDIENTE
        propuesta.Estado = "PENDIENTE";

        // Asegurar que todos los DateTime sean UTC antes de guardar (requerido para PostgreSQL)
        if (propuesta.FechaCreacion.Kind == DateTimeKind.Unspecified)
            propuesta.FechaCreacion = DateTime.SpecifyKind(propuesta.FechaCreacion, DateTimeKind.Utc);

        if (propuesta.FechaEnvioRevision.HasValue && propuesta.FechaEnvioRevision.Value.Kind == DateTimeKind.Unspecified)
            propuesta.FechaEnvioRevision = DateTime.SpecifyKind(propuesta.FechaEnvioRevision.Value, DateTimeKind.Utc);

        propuesta.FechaActualizacion = FechaEcuador.Ahora();

        // Limpiar observaciones
        if (propuesta.Observaciones != null && propuesta.Observaciones.Any())
        {
            propuesta.Observaciones.Clear();
        }

        var actualizada = await _repository.UpdateAsync(propuesta);
        return _mapper.Map<PropuestaDto>(actualizada);
    }

    // ===== ASIGNATURAS =====

    /// <summary>
    /// Asigna una asignatura a una propuesta
    /// </summary>
    public async Task<bool> AsignarAsignaturaAsync(int propuestaId, int asignaturaId)
    {
        if (propuestaId <= 0 || asignaturaId <= 0)
            throw new ArgumentException("IDs invÃ¡lidos");

        // Verificar que propuesta y asignatura existan
        var propuesta = await _repository.GetByIdAsync(propuestaId);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta {propuestaId} no encontrada");

        var asignatura = await _asignaturaRepository.GetByIdAsync(asignaturaId);
        if (asignatura == null)
            throw new InvalidOperationException($"Asignatura {asignaturaId} no encontrada");

        // Verificar que no estÃ© ya asignada
        if (propuesta.PropuestasAsignaturas.Any(pa => pa.AsignaturaId == asignaturaId))
            return false; // Ya asignada

        // Agregar nueva relaciÃ³n
        propuesta.PropuestasAsignaturas.Add(new PropuestaAsignatura
        {
            PropuestaId = propuestaId,
            AsignaturaId = asignaturaId
        });

        propuesta.FechaActualizacion = FechaEcuador.Ahora();
        await _repository.UpdateAsync(propuesta);
        return true;
    }

    /// <summary>
    /// Remueve una asignatura de una propuesta
    /// </summary>
    public async Task<bool> RemoverAsignaturaAsync(int propuestaId, int asignaturaId)
    {
        if (propuestaId <= 0 || asignaturaId <= 0)
            throw new ArgumentException("IDs invÃ¡lidos");

        var propuesta = await _repository.GetByIdAsync(propuestaId);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta {propuestaId} no encontrada");

        var asignacion = propuesta.PropuestasAsignaturas
            .FirstOrDefault(pa => pa.AsignaturaId == asignaturaId);

        if (asignacion == null)
            return false; // No encontrada

        propuesta.PropuestasAsignaturas.Remove(asignacion);
        propuesta.FechaActualizacion = FechaEcuador.Ahora();
        await _repository.UpdateAsync(propuesta);
        return true;
    }

    /// <summary>
    /// HU07 T23: Solicita una nueva aprobaciÃ³n de propuesta aprobada
    /// Cambia estado APROBADA â†’ PENDIENTE
    /// Solo se puede hacer desde APROBADA
    /// Registra cambio en historial de estados
    /// </summary>
    public async Task<PropuestaDto> SolicitarNuevaAprobacionAsync(int id, SolicitarNuevaAprobacionDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID invalido", nameof(id));

        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Motivo))
            throw new ArgumentException("Motivo es requerido", nameof(dto.Motivo));

        var propuesta = await _repository.GetPropuestaFullAsync(id);
        if (propuesta == null)
            throw new InvalidOperationException($"Propuesta con ID {id} no encontrada");

        if (propuesta.Estado != "PENDIENTE")
            throw new InvalidOperationException("Solo PENDIENTE puede solicitar nueva aprobacion");

        var tieneEstudiantesAsignados = propuesta.Componentes != null &&
            propuesta.Componentes.Any(componente => componente.EstudianteId.HasValue);

        if (!tieneEstudiantesAsignados)
            throw new InvalidOperationException("La propuesta debe tener estudiantes asignados");

        var ahora = FechaEcuador.Ahora();
        const string prefijoObservacionNuevaAprobacion = "Observacion para el miembro de la CPGIC:";
        propuesta.FechaActualizacion = ahora;
        propuesta.FechaEnvioRevision = ahora;
        var observacionesPrevias = propuesta.Observaciones
            .Where(o => o.Observacion.StartsWith(prefijoObservacionNuevaAprobacion) ||
                o.Observacion.StartsWith("Solicitud de nueva aprobacion:"))
            .ToList();

        foreach (var observacionPrevia in observacionesPrevias)
        {
            propuesta.Observaciones.Remove(observacionPrevia);
        }

        propuesta.Observaciones.Add(new ObservacionesCpgic
        {
            PropuestaId = propuesta.Id,
            Observacion = $"{prefijoObservacionNuevaAprobacion} esta propuesta ya fue aprobada previamente. Revisar solo el nuevo cambio registrado en la asignacion o actualizacion de estudiantes. Motivo: {dto.Motivo.Trim()}",
            RealizadoPor = "Docente",
            FechaObservacion = ahora
        });

        var actualizada = await _repository.UpdateAsync(propuesta);
        return _mapper.Map<PropuestaDto>(actualizada);
    }
}
