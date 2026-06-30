using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Application.Mappings;
using TesisTIC.Application.Services;
using TesisTIC.Infrastructure.Data;
using TesisTIC.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(PropuestaMappingProfile).Assembly);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("TesisTICConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Database connection string is not configured. Set ConnectionStrings__TesisTICConnection as an environment variable, use dotnet user-secrets, or create TesisTIC.API/appsettings.Local.json.");
}

builder.Services.AddDbContext<TesisTicDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});

builder.Services.AddScoped<IPropuestaRepository, PropuestaRepository>();
builder.Services.AddScoped<IAsignaturaRepository, AsignaturaRepository>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IObservacionesCpgicRepository, ObservacionesCpgicRepository>();
builder.Services.AddScoped<IHistorialEstadoRepository, HistorialEstadoRepository>();
builder.Services.AddScoped<IPropuestaEstudianteRepository, PropuestaEstudianteRepository>();

builder.Services.AddScoped<IPropuestaService, PropuestaService>();
builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IObservacionesCpgicService, ObservacionesCpgicService>();
builder.Services.AddScoped<IPropuestaDetalleService, PropuestaDetalleService>();
builder.Services.AddScoped<IPropuestaEstudianteService, PropuestaEstudianteService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>();
        var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>();
        
        policyBuilder
            .WithOrigins(allowedOrigins ?? new[] { "http://localhost:4200" })
            .WithMethods(allowedMethods ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" })
            .WithHeaders(allowedHeaders ?? new[] { "*" })
            .AllowCredentials();
    });
});

var app = builder.Build();

// ✅ CORS DEBE IR ANTES DE OTROS MIDDLEWARE
app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TesisTicDbContext>();
        dbContext.Database.Migrate();
        // DbInitializer.Initialize(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        Console.WriteLine($"Error details: {ex}");
        Console.WriteLine("Continuing without database initialization...");
    }
}

app.Run();
