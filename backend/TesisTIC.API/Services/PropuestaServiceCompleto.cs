using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;
using TesisTIC.Infrastructure.Persistence;

namespace TesisTIC.API.Services
{
    public class PropuestaServiceCompleto : IPropuestaService
    {
        private readonly TesisTICDbContext _context;

        public PropuestaServiceCompleto(TesisTICDbContext context)
        {
            _context = context;
        }

        public async Task<PropuestaDetailDto> ObtenerPropuestaAsync(int id)
        {
            var propuesta = await _context.Propuestas
                .Include(p => p.Docente)
                .Include(p => p.Estado)
                .Include(p => p.AsignaturasAsignadas).ThenInclude(pa => pa.Asignatura)
                .Include(p => p.Modulos).ThenInclude(m => m.Actividades)
                .Include(p => p.Modulos).ThenInclude(m => m.Observaciones)
                .Include(p => p.Observaciones)
                .Include(p => p.Rechazo).ThenInclude(r => r.Razones)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propuesta == null)
                throw new KeyNotFoundException($"Propuesta no encontrada");

            return MapearADetailDto(propuesta);
        }

        public async Task<IEnumerable<ListaPropuestasDto>> ObtenerTodasAsync()
        {
            var propuestas = await _context.Propuestas
                .Include(p => p.Estado)
                .Include(p => p.Modulos)
                .Include(p => p.Observaciones)
                .Include(p => p.Rechazo)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            return propuestas.Select(p => MapearAListaDto(p)).ToList();
        }

        public async Task<IEnumerable<ListaPropuestasDto>> ObtenerPorDocenteAsync(int docenteId)
        {
            var propuestas = await _context.Propuestas
                .Where(p => p.DocenteId == docenteId)
                .Include(p => p.Estado)
                .Include(p => p.Modulos)
                .Include(p => p.Observaciones)
                .Include(p => p.Rechazo)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            return propuestas.Select(p => MapearAListaDto(p)).ToList();
        }

        public async Task<IEnumerable<ListaPropuestasDto>> ObtenerPorEstadoAsync(string estado)
        {
            var propuestas = await _context.Propuestas
                .Include(p => p.Estado)
                .Include(p => p.Modulos)
                .Include(p => p.Observaciones)
                .Include(p => p.Rechazo)
                .Where(p => p.Estado.Nombre == estado)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            return propuestas.Select(p => MapearAListaDto(p)).ToList();
        }

        public async Task<EstadisticasDto> ObtenerEstadisticasAsync(int docenteId)
        {
            var propuestas = await _context.Propuestas
                .Include(p => p.Estado)
                .Where(p => p.DocenteId == docenteId)
                .ToListAsync();

            return new EstadisticasDto
            {
                Todos = propuestas.Count,
                Borrador = propuestas.Count(p => p.Estado.Nombre == "Borrador"),
                Pendiente = propuestas.Count(p => p.Estado.Nombre == "Pendiente"),
                Observada = propuestas.Count(p => p.Estado.Nombre == "Observada"),
                Aprobada = propuestas.Count(p => p.Estado.Nombre == "Aprobada"),
                Rechazada = propuestas.Count(p => p.Estado.Nombre == "Rechazada")
            };
        }

        public async Task<PropuestaDetailDto> CrearPropuestaAsync(GuardarPropuestaDto dto, int docenteId)
        {
            var estado = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Borrador");

            var propuesta = new Propuesta
            {
                Titulo = dto.Titulo,
                Proyecto = dto.Proyecto,
                Descripcion = dto.Descripcion,
                Objetivo = dto.Objetivo,
                Alcance = dto.Alcance,
                NumeroParticipantes = dto.NumeroParticipantes,
                DocenteId = docenteId,
                EstadoId = estado.Id,
                Facultad = "Facultad de Ingeniería de Sistemas",
                Departamento = "Depto. de Informática y Ciencias de la Computación",
                FechaCreacion = DateTime.UtcNow
            };

            _context.Propuestas.Add(propuesta);
            await _context.SaveChangesAsync();

            // Agregar asignaturas
            foreach (var sigla in dto.Asignaturas)
            {
                var asignatura = await _context.Asignaturas.FirstOrDefaultAsync(a => a.Nombre == sigla);
                if (asignatura == null)
                {
                    asignatura = new Asignatura { Nombre = sigla, Codigo = GenerarCodigo(sigla) };
                    _context.Asignaturas.Add(asignatura);
                    await _context.SaveChangesAsync();
                }

                _context.PropuestasAsignaturas.Add(new PropuestaAsignatura
                {
                    PropuestaId = propuesta.Id,
                    AsignaturaId = asignatura.Id
                });
            }

            // Agregar módulos
            foreach (var moduloDto in dto.Modulos)
            {
                var modulo = new Modulo
                {
                    PropuestaId = propuesta.Id,
                    NumeroModulo = moduloDto.NumeroModulo,
                    Nombre = moduloDto.Nombre,
                    Descripcion = moduloDto.Descripcion,
                    Productos = moduloDto.Productos
                };

                _context.Modulos.Add(modulo);
                await _context.SaveChangesAsync();

                foreach (var actividadDto in moduloDto.Actividades)
                {
                    _context.Actividades.Add(new Actividad
                    {
                        ModuloId = modulo.Id,
                        Descripcion = actividadDto.Descripcion,
                        Horas = actividadDto.Horas
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await ObtenerPropuestaAsync(propuesta.Id);
        }

        public async Task<PropuestaDetailDto> ActualizarPropuestaAsync(int id, GuardarPropuestaDto dto)
        {
            var propuesta = await _context.Propuestas
                .Include(p => p.Modulos)
                .Include(p => p.AsignaturasAsignadas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propuesta == null)
                throw new KeyNotFoundException("Propuesta no encontrada");

            propuesta.Titulo = dto.Titulo;
            propuesta.Proyecto = dto.Proyecto;
            propuesta.Descripcion = dto.Descripcion;
            propuesta.Objetivo = dto.Objetivo;
            propuesta.Alcance = dto.Alcance;
            propuesta.NumeroParticipantes = dto.NumeroParticipantes;
            propuesta.FechaActualizacion = DateTime.UtcNow;

            // Actualizar asignaturas
            var asignaturasActuales = _context.PropuestasAsignaturas.Where(pa => pa.PropuestaId == id).ToList();
            _context.PropuestasAsignaturas.RemoveRange(asignaturasActuales);

            foreach (var sigla in dto.Asignaturas)
            {
                var asignatura = await _context.Asignaturas.FirstOrDefaultAsync(a => a.Nombre == sigla);
                if (asignatura == null)
                {
                    asignatura = new Asignatura { Nombre = sigla, Codigo = GenerarCodigo(sigla) };
                    _context.Asignaturas.Add(asignatura);
                    await _context.SaveChangesAsync();
                }

                _context.PropuestasAsignaturas.Add(new PropuestaAsignatura
                {
                    PropuestaId = id,
                    AsignaturaId = asignatura.Id
                });
            }

            await _context.SaveChangesAsync();
            return await ObtenerPropuestaAsync(id);
        }

        public async Task<bool> EliminarPropuestaAsync(int id)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                return false;

            _context.Propuestas.Remove(propuesta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PropuestaDetailDto> AsignarEstudiantesAsync(int id, AsignarEstudiantesDto dto)
        {
            var propuesta = await _context.Propuestas
                .Include(p => p.Modulos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (propuesta == null)
                throw new KeyNotFoundException("Propuesta no encontrada");

            foreach (var asignacion in dto.Modulos)
            {
                var modulo = propuesta.Modulos.FirstOrDefault(m => m.Id == asignacion.ModuloId);
                if (modulo != null)
                {
                    modulo.EstudianteAsignado = asignacion.EstudianteNombre;
                    modulo.FechaModificacion = DateTime.UtcNow;
                }
            }

            propuesta.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return await ObtenerPropuestaAsync(id);
        }

        public async Task<PropuestaDetailDto> CambiarEstadoAsync(int id, string estado)
        {
            var propuesta = await _context.Propuestas.FindAsync(id);
            if (propuesta == null)
                throw new KeyNotFoundException("Propuesta no encontrada");

            var nuevoEstado = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == estado);
            if (nuevoEstado == null)
                throw new ArgumentException("Estado no válido");

            propuesta.EstadoId = nuevoEstado.Id;
            propuesta.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await ObtenerPropuestaAsync(id);
        }

        private ListaPropuestasDto MapearAListaDto(Propuesta p)
        {
            return new ListaPropuestasDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Estado = p.Estado?.Nombre ?? "Desconocido",
                Fecha = p.FechaActualizacion ?? p.FechaCreacion,
                Proyecto = p.Proyecto,
                TieneObservaciones = p.Observaciones.Any() || p.Modulos.Any(m => m.Observaciones.Any()),
                ModulosSinEstudiante = p.Modulos.Count(m => string.IsNullOrEmpty(m.EstudianteAsignado))
            };
        }

        private PropuestaDetailDto MapearADetailDto(Propuesta p)
        {
            return new PropuestaDetailDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Proyecto = p.Proyecto,
                Descripcion = p.Descripcion,
                Objetivo = p.Objetivo,
                Alcance = p.Alcance,
                NumeroParticipantes = p.NumeroParticipantes,
                Departamento = p.Departamento,
                Facultad = p.Facultad,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion,
                Estado = p.Estado?.Nombre ?? "Desconocido",
                Asignaturas = p.AsignaturasAsignadas?.Select(pa => pa.Asignatura?.Nombre).ToList() ?? new(),
                Modulos = p.Modulos?.Select(m => new ModuloDto
                {
                    Id = m.Id,
                    NumeroModulo = m.NumeroModulo,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    Productos = m.Productos,
                    EstudianteAsignado = m.EstudianteAsignado,
                    Actividades = m.Actividades?.Select(a => new ActividadDto
                    {
                        Id = a.Id,
                        Descripcion = a.Descripcion,
                        Horas = a.Horas
                    }).ToList() ?? new(),
                    Observaciones = m.Observaciones?.FirstOrDefault() != null ? new ObservacionModuloDto
                    {
                        Id = m.Observaciones.First().Id,
                        Autor = m.Observaciones.First().Autor,
                        Texto = m.Observaciones.First().Texto,
                        Fecha = m.Observaciones.First().Fecha
                    } : null
                }).ToList() ?? new(),
                ObservacionDescripcion = p.Observaciones?.FirstOrDefault() != null ? new ObservacionPropuestaDto
                {
                    Id = p.Observaciones.First().Id,
                    Autor = p.Observaciones.First().Autor,
                    Texto = p.Observaciones.First().Texto,
                    Fecha = p.Observaciones.First().Fecha
                } : null,
                Rechazo = p.Rechazo != null ? new RechazoPropuestaDto
                {
                    Id = p.Rechazo.Id,
                    Autor = p.Rechazo.Autor,
                    Fecha = p.Rechazo.Fecha,
                    Razones = p.Rechazo.Razones?.Select(r => r.Razon).ToList() ?? new(),
                    Recomendacion = p.Rechazo.Recomendacion
                } : null
            };
        }

        private string GenerarCodigo(string nombre)
        {
            // Generar un código simple basado en el nombre
            var palabras = nombre.Split(' ');
            return string.Concat(palabras.Select(p => char.ToUpper(p[0])));
        }
    }
}
