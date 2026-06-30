using AutoMapper;
using TesisTIC.Application.DTOs;
using TesisTIC.Application.Interfaces;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Application.Services;

/// <summary>
/// Servicio para operaciones con Docentes
/// </summary>
public class DocenteService : IDocenteService
{
    private readonly IDocenteRepository _repository;
    private readonly IMapper _mapper;

    public DocenteService(IDocenteRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<DocenteDto>> GetAllAsync()
    {
        var docentes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<DocenteDto>>(docentes);
    }

    public async Task<DocenteDto?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        var docente = await _repository.GetByIdAsync(id);
        return docente != null ? _mapper.Map<DocenteDto>(docente) : null;
    }

    public async Task<DocenteDto> CreateAsync(CreateUpdateDocenteDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Nombres) || string.IsNullOrWhiteSpace(dto.Apellidos))
            throw new ArgumentException("Nombres y apellidos son requeridos");

        var docente = new Docente
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Correo = dto.Correo,
            TituloAcademico = dto.TituloAcademico,
            FechaCreacion = DateTime.UtcNow
        };

        var creado = await _repository.CreateAsync(docente);
        return _mapper.Map<DocenteDto>(creado);
    }

    public async Task<DocenteDto> UpdateAsync(int id, CreateUpdateDocenteDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var docente = await _repository.GetByIdAsync(id);
        if (docente == null)
            throw new InvalidOperationException($"Docente con ID {id} no encontrado");

        docente.Nombres = dto.Nombres;
        docente.Apellidos = dto.Apellidos;
        docente.Correo = dto.Correo;
        docente.TituloAcademico = dto.TituloAcademico;

        var actualizado = await _repository.UpdateAsync(docente);
        return _mapper.Map<DocenteDto>(actualizado);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID inválido", nameof(id));

        return await _repository.DeleteAsync(id);
    }
}
