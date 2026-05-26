using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Application.Services;
using TesisTIC.Infrastructure.Data;
using TesisTIC.Infrastructure.Repositories;
using TesisTIC.Application.Mappings;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// ===== SERVICIOS =====

// Agregar DbContext con PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TesisTicDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
    maxRetryCount: 3,
    maxRetryDelay: TimeSpan.FromSeconds(10),
    errorCodesToAdd: null);
    }));

// ===== REGISTRAR REPOSITORIOS =====
builder.Services.AddScoped<IPropuestaRepository, PropuestaRepository>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IAsignaturaRepository, AsignaturaRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IObservacionesCpgicRepository, ObservacionesCpgicRepository>(); // HU04 T10
builder.Services.AddScoped<IHistorialEstadoRepository, HistorialEstadoRepository>(); // HU06 T17
builder.Services.AddScoped<IPropuestaEstudianteRepository, PropuestaEstudianteRepository>(); // HU07 T20

// ===== REGISTRAR SERVICIOS =====
builder.Services.AddScoped<IPropuestaService, PropuestaService>();
builder.Services.AddScoped<IPropuestaDetalleService, PropuestaDetalleService>(); // HU06 T17
builder.Services.AddScoped<IPropuestaEstudianteService, PropuestaEstudianteService>(); // HU07 T20
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IObservacionesCpgicService, ObservacionesCpgicService>(); // HU04 T10

// ===== REGISTRAR AUTOMAPPER =====
// Resolver AutoMapper manualmente
var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new TesisTIC.Application.Mappings.PropuestaMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.DocenteMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.AsignaturaMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.EstudianteMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.ComponenteMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.ActividadMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.ProductoEsperadoMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.ObservacionesCpgicMappingProfile());
    cfg.AddProfile(new TesisTIC.Application.Mappings.AprobacionesCpgicMappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Agregar controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar JSON serialization para propiedades null
        options.JsonSerializerOptions.WriteIndented = true;
        // Permitir nombres de propiedad en lowercase (camelCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Agregar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TesisTIC API",
        Version = "v1.0",
        Description = "API para gestión de propuestas TIC de la Escuela Politécnica Nacional"
    });
});

// Configurar CORS para Angular
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ===== CONSTRUIR APLICACIÓN =====
var app = builder.Build();

// ===== MIDDLEWARE =====

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TesisTIC API v1");
    c.RoutePrefix = string.Empty;
});

// Health check endpoint
app.MapGet("/health", () =>
{
    return Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
})
.WithName("Health Check");

app.UseHttpsRedirection();

// Aplicar CORS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
