using Microsoft.EntityFrameworkCore;
using TesisTIC.Application.Interfaces;
using TesisTIC.Application.Services;
using TesisTIC.Infrastructure.Persistence;
using TesisTIC.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("TesisTICConnection");
builder.Services.AddDbContext<TesisTICDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddScoped<IPropuestaRepository, PropuestaRepository>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
builder.Services.AddScoped<ILineaInvestigacionRepository, LineaInvestigacionRepository>();
builder.Services.AddScoped<IAsignaturaRepository, AsignaturaRepository>();

builder.Services.AddScoped<IPropuestaService, PropuestaService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        policyBuilder.WithOrigins(allowedOrigins ?? new[] { "http://localhost:4200" })
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TesisTICDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
        Console.WriteLine("Continuing without database initialization...");
    }
}

app.Run();
