using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services
{
    public class PropuestaService : IPropuestaService
    {
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IEstudianteRepository _estudianteRepository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly ILineaInvestigacionRepository _lineaRepository;
        private readonly IAsignaturaRepository _asignaturaRepository;
        private readonly IMapper _mapper;

        public PropuestaService(
            IPropuestaRepository propuestaRepository,
            IDocenteRepository docenteRepository,
            IEstudianteRepository estudianteRepository,
            IEstadoRepository estadoRepository,
            ILineaInvestigacionRepository lineaRepository,
            IAsignaturaRepository asignaturaRepository,
            IMapper mapper)
        {
            _propuestaRepository = propuestaRepository;
            _docenteRepository = docenteRepository;
            _estudianteRepository = estudianteRepository;
            _estadoRepository = estadoRepository;
            _lineaRepository = lineaRepository;
            _asignaturaRepository = asignaturaRepository;
            _mapper = mapper;
        }

        public async Task<PropuestaDto> CrearPropuestaAsync(CrearPropuestaDto dto)
        {
            ValidarCrearPropuesta(dto);

            var docente = await _docenteRepository.ObtenerPorIdAsync(dto.DocenteId);
            if (docente == null)
                throw new ArgumentException("El docente especificado no existe.");

            var estadoPendiente = await _estadoRepository.ObtenerPorNombreAsync("Pendiente");
            if (estadoPendiente == null)
                throw new InvalidOperationException("No existe el estado predeterminado 'Pendiente'.");

            var linea = dto.LineaInvestigacionId.HasValue ? 
                await _lineaRepository.ObtenerPorIdAsync(dto.LineaInvestigacionId.Value) : null;

            var propuesta = new Propuesta
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Objetivo = dto.Objetivo,
                Alcance = dto.Alcance,
                ComponentesActividadesProductos = dto.ComponentesActividadesProductos,
                DocenteId = dto.DocenteId,
                EstadoId = estadoPendiente.Id,
                LineaInvestigacionId = dto.LineaInvestigacionId,
                NumeroParticipantes = dto.NumeroParticipantes,
                Departamento = dto.Departamento,
                Facultad = dto.Facultad
            };

            if (dto.AsignaturasIds.Any())
            {
                var asignaturas = new List<Asignatura>();
                foreach (var asignaturaId in dto.AsignaturasIds)
                {
                    var asignatura = await _asignaturaRepository.ObtenerPorIdAsync(asignaturaId);
                    if (asignatura != null)
                        asignaturas.Add(asignatura);
                }
                propuesta.Asignaturas = asignaturas;
            }

            var propuestaCreada = await _propuestaRepository.CrearAsync(propuesta);
            return MapearAPropuestaDto(propuestaCreada);
        }

        public async Task<PropuestaDto> ObtenerPropuestaAsync(int id)
        {
            var propuesta = await _propuestaRepository.ObtenerPorIdAsync(id);
            if (propuesta == null)
                throw new KeyNotFoundException($"No existe propuesta con ID {id}.");

            return MapearAPropuestaDto(propuesta);
        }

        public async Task<IEnumerable<PropuestaDto>> ObtenerTodasAsync()
        {
            var propuestas = await _propuestaRepository.ObtenerTodasAsync();
            return propuestas.Select(MapearAPropuestaDto);
        }

        public async Task<IEnumerable<PropuestaDto>> ObtenerPorDocenteAsync(int docenteId)
        {
            var docente = await _docenteRepository.ObtenerPorIdAsync(docenteId);
            if (docente == null)
                throw new ArgumentException("El docente especificado no existe.");

            var propuestas = await _propuestaRepository.ObtenerPorDocenteAsync(docenteId);
            return propuestas.Select(MapearAPropuestaDto);
        }

        public async Task<IEnumerable<PropuestaDto>> ObtenerPorEstadoAsync(int estadoId)
        {
            var estado = await _estadoRepository.ObtenerPorIdAsync(estadoId);
            if (estado == null)
                throw new ArgumentException("El estado especificado no existe.");

            var propuestas = await _propuestaRepository.ObtenerPorEstadoAsync(estadoId);
            return propuestas.Select(MapearAPropuestaDto);
        }

        public async Task<PropuestaDto> ActualizarPropuestaAsync(ActualizarPropuestaDto dto)
        {
            var propuesta = await _propuestaRepository.ObtenerPorIdAsync(dto.Id);
            if (propuesta == null)
                throw new KeyNotFoundException($"No existe propuesta con ID {dto.Id}.");

            propuesta.Titulo = dto.Titulo;
            propuesta.Descripcion = dto.Descripcion;
            propuesta.Objetivo = dto.Objetivo;
            propuesta.Alcance = dto.Alcance;
            propuesta.ComponentesActividadesProductos = dto.ComponentesActividadesProductos;
            propuesta.LineaInvestigacionId = dto.LineaInvestigacionId;
            propuesta.Observaciones = dto.Observaciones;
            propuesta.NumeroParticipantes = dto.NumeroParticipantes;
            propuesta.Departamento = dto.Departamento;
            propuesta.Facultad = dto.Facultad;

            if (dto.AsignaturasIds.Any())
            {
                propuesta.Asignaturas.Clear();
                foreach (var asignaturaId in dto.AsignaturasIds)
                {
                    var asignatura = await _asignaturaRepository.ObtenerPorIdAsync(asignaturaId);
                    if (asignatura != null)
                        propuesta.Asignaturas.Add(asignatura);
                }
            }

            var propuestaActualizada = await _propuestaRepository.ActualizarAsync(propuesta);
            return MapearAPropuestaDto(propuestaActualizada);
        }

        public async Task<bool> CambiarEstadoAsync(CambiarEstadoDto dto)
        {
            var propuesta = await _propuestaRepository.ObtenerPorIdAsync(dto.PropuestaId);
            if (propuesta == null)
                throw new KeyNotFoundException($"No existe propuesta con ID {dto.PropuestaId}.");

            var nuevoEstado = await _estadoRepository.ObtenerPorIdAsync(dto.EstadoId);
            if (nuevoEstado == null)
                throw new ArgumentException("El estado especificado no existe.");

            propuesta.EstadoId = dto.EstadoId;
            propuesta.Observaciones = dto.Observaciones;

            if (dto.EstadoId == 2)
                propuesta.FechaEnvioPrimera = DateTime.UtcNow;

            await _propuestaRepository.ActualizarAsync(propuesta);
            return true;
        }

        public async Task<bool> AsignarEstudianteAsync(AsignarEstudianteDto dto)
        {
            var propuesta = await _propuestaRepository.ObtenerPorIdAsync(dto.PropuestaId);
            if (propuesta == null)
                throw new KeyNotFoundException($"No existe propuesta con ID {dto.PropuestaId}.");

            var estudiante = await _estudianteRepository.ObtenerPorIdAsync(dto.EstudianteId);
            if (estudiante == null)
                throw new ArgumentException("El estudiante especificado no existe.");

            var yaAsignado = propuesta.EstudiantesAsignados
                .Any(pe => pe.EstudianteId == dto.EstudianteId);

            if (yaAsignado)
                throw new InvalidOperationException("El estudiante ya está asignado a esta propuesta.");

            var propuestaEstudiante = new PropuestaEstudiante
            {
                PropuestaId = dto.PropuestaId,
                EstudianteId = dto.EstudianteId,
                FechaAsignacion = DateTime.UtcNow
            };

            propuesta.EstudiantesAsignados.Add(propuestaEstudiante);
            await _propuestaRepository.ActualizarAsync(propuesta);
            return true;
        }

        public async Task<bool> EliminarPropuestaAsync(int id)
        {
            return await _propuestaRepository.EliminarAsync(id);
        }

        private void ValidarCrearPropuesta(CrearPropuestaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ArgumentException("La descripción es requerida.");

            if (string.IsNullOrWhiteSpace(dto.Objetivo))
                throw new ArgumentException("El objetivo es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Alcance))
                throw new ArgumentException("El alcance es requerido.");

            if (string.IsNullOrWhiteSpace(dto.ComponentesActividadesProductos))
                throw new ArgumentException("Los componentes, actividades y productos son requeridos.");

            if (dto.NumeroParticipantes <= 0)
                throw new ArgumentException("El número de participantes debe ser mayor a cero.");
        }

        private PropuestaDto MapearAPropuestaDto(Propuesta propuesta)
        {
            return new PropuestaDto
            {
                Id = propuesta.Id,
                Titulo = propuesta.Titulo,
                Descripcion = propuesta.Descripcion,
                Objetivo = propuesta.Objetivo,
                Alcance = propuesta.Alcance,
                ComponentesActividadesProductos = propuesta.ComponentesActividadesProductos,
                DocenteId = propuesta.DocenteId,
                DocenteNombre = $"{propuesta.Docente?.Nombre} {propuesta.Docente?.Apellido}",
                EstadoId = propuesta.EstadoId,
                EstadoNombre = propuesta.Estado?.Nombre,
                LineaInvestigacionId = propuesta.LineaInvestigacionId,
                LineaInvestigacionNombre = propuesta.LineaInvestigacion?.Nombre,
                Observaciones = propuesta.Observaciones,
                NumeroParticipantes = propuesta.NumeroParticipantes,
                Departamento = propuesta.Departamento,
                Facultad = propuesta.Facultad,
                FechaCreacion = propuesta.FechaCreacion,
                FechaActualizacion = propuesta.FechaActualizacion,
                FechaEnvioPrimera = propuesta.FechaEnvioPrimera,
                EstudiantesAsignados = propuesta.EstudiantesAsignados.Select(pe => new EstudianteAsignadoDto
                {
                    Id = pe.Id,
                    EstudianteId = pe.EstudianteId,
                    NombreEstudiante = pe.Estudiante?.Nombre,
                    ApellidoEstudiante = pe.Estudiante?.Apellido,
                    MatriculaEstudiante = pe.Estudiante?.Matricula,
                    FechaAsignacion = pe.FechaAsignacion
                }).ToList(),
                Asignaturas = propuesta.Asignaturas.Select(a => new AsignaturaDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Codigo = a.Codigo
                }).ToList()
            };
        }
    }
}
