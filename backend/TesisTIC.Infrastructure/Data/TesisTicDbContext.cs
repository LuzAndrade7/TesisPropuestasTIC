using Microsoft.EntityFrameworkCore;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Infrastructure.Data;

/// <summary>
/// DbContext principal que configura el acceso a datos con PostgreSQL
/// Contiene los DbSets para todas las entidades del sistema
/// </summary>
public class TesisTicDbContext : DbContext
{
    public TesisTicDbContext(DbContextOptions<TesisTicDbContext> options) : base(options)
    {
    }

    // DbSets - Tablas principales del sistema
    public DbSet<Docente> Docentes { get; set; } = null!;
    public DbSet<Propuesta> Propuestas { get; set; } = null!;
    public DbSet<Asignatura> Asignaturas { get; set; } = null!;
    public DbSet<PropuestaAsignatura> PropuestasAsignaturas { get; set; } = null!;
    public DbSet<Estudiante> Estudiantes { get; set; } = null!;
    public DbSet<Componente> Componentes { get; set; } = null!;
    public DbSet<Actividad> Actividades { get; set; } = null!;
    public DbSet<ProductoEsperado> ProductosEsperados { get; set; } = null!;
    public DbSet<ObservacionesCpgic> Observaciones { get; set; } = null!;
    public DbSet<AprobacionesCpgic> Aprobaciones { get; set; } = null!;
    public DbSet<HistorialEstado> HistorialEstados { get; set; } = null!; // HU06 T17    public DbSet<PropuestaEstudiante> PropuestaEstudiantes { get; set; } = null!; // HU07 T20
    /// <summary>
    /// Configura el mapeo entre entidades y tablas de base de datos
    /// Establece las restricciones, relaciones y índices
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== CONVENCIÓN PARA POSTGRESQL =====
        // Convertir automáticamente PascalCase a snake_case para nombres de columnas
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convertir nombres de columnas a snake_case
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (columnName == property.Name || columnName == property.Name[0].ToString().ToUpper() + property.Name[1..])
                {
                    property.SetColumnName(ConvertToSnakeCase(property.Name));
                }
            }
        }

        // ===== TABLA DOCENTES =====
        modelBuilder.Entity<Docente>(entity =>
        {
            entity.ToTable("docentes", "public");
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Nombres)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Apellidos)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Correo)
                .HasMaxLength(150);

            entity.Property(d => d.TituloAcademico)
                .HasMaxLength(100);

            entity.Property(d => d.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Índices
            entity.HasIndex(d => d.Correo).IsUnique(false);
        });

        // ===== TABLA ASIGNATURAS =====
        modelBuilder.Entity<Asignatura>(entity =>
        {
            entity.ToTable("asignaturas", "public");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Codigo)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(a => a.Codigo).IsUnique();

            entity.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(150);
        });

        // ===== TABLA PROPUESTAS =====
        modelBuilder.Entity<Propuesta>(entity =>
        {
            entity.ToTable("propuestas", "public");
            entity.HasKey(p => p.Id);

            entity.Ignore(p => p.UnidadAcademica);
            entity.Ignore(p => p.Carrera);
            entity.Ignore(p => p.LineaInvestigacionId);
            entity.Ignore(p => p.LineaInvestigacion);
            entity.Ignore(p => p.DepartmentId);
            entity.Ignore(p => p.Department);
            entity.Ignore(p => p.FechaAprobacion);
            entity.Ignore(p => p.FechaRechazo);

            entity.Property(p => p.NombreProyecto)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(p => p.NumeroParticipantes)
                .IsRequired();

            entity.Property(p => p.ProfesorId)
                .IsRequired();

            entity.Property(p => p.Descripcion)
                .IsRequired();

            entity.Property(p => p.Objetivo)
                .IsRequired();

            entity.Property(p => p.Alcance)
                .IsRequired();

            entity.Property(p => p.Estado)
                .HasMaxLength(50)
                .HasDefaultValue("BORRADOR");

            entity.Property(p => p.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(p => p.FechaActualizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relación con Docente
            entity.HasOne(p => p.Profesor)
                .WithMany(d => d.Propuestas)
                .HasForeignKey(p => p.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_propuestas_docentes");

            // Índices
            entity.HasIndex(p => p.ProfesorId);
            entity.HasIndex(p => p.Estado);
        });

        // ===== TABLA PROPUESTA_ASIGNATURAS =====
        modelBuilder.Entity<PropuestaAsignatura>(entity =>
        {
            entity.ToTable("propuesta_asignaturas", "public");
            entity.HasKey(pa => pa.Id);

            entity.Property(pa => pa.PropuestaId).IsRequired();
            entity.Property(pa => pa.AsignaturaId).IsRequired();

            // Relación con Propuesta
            entity.HasOne(pa => pa.Propuesta)
                .WithMany(p => p.PropuestasAsignaturas)
                .HasForeignKey(pa => pa.PropuestaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_propuesta_asignaturas_propuestas");

            // Relación con Asignatura
            entity.HasOne(pa => pa.Asignatura)
                .WithMany(a => a.PropuestasAsignaturas)
                .HasForeignKey(pa => pa.AsignaturaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_propuesta_asignaturas_asignaturas");

            // Única combinación de propuesta y asignatura
            entity.HasIndex(pa => new { pa.PropuestaId, pa.AsignaturaId })
                .IsUnique()
                .HasDatabaseName("uq_propuesta_asignatura");
        });

        // ===== TABLA ESTUDIANTES =====
        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.ToTable("estudiantes", "public");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.NombresEstudiante)
                .HasColumnName("nombres_estudiante")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ===== TABLA COMPONENTES =====
        modelBuilder.Entity<Componente>(entity =>
        {
            entity.ToTable("componentes", "public");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.PropuestaId).IsRequired();
            entity.Property(c => c.Nombre).HasMaxLength(150);
            entity.Property(c => c.Descripcion).IsRequired();
            entity.Property(c => c.Orden).IsRequired();

            // Relación con Propuesta
            entity.HasOne(c => c.Propuesta)
                .WithMany(p => p.Componentes)
                .HasForeignKey(c => c.PropuestaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_componentes_propuestas");

            // Relación con Estudiante (opcional)
            entity.HasOne(c => c.Estudiante)
                .WithMany(e => e.Componentes)
                .HasForeignKey(c => c.EstudianteId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_componentes_estudiantes");

            entity.HasIndex(c => c.EstudianteId).IsUnique(false);
        });

        // ===== TABLA ACTIVIDADES =====
        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.ToTable("actividades", "public");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.ComponenteId).HasColumnName("componente_id").IsRequired();
            entity.Property(a => a.Numero).IsRequired();
            entity.Property(a => a.Descripcion).IsRequired();
            entity.Property(a => a.Horas).IsRequired();

            // Relación con Componente
            entity.HasOne(a => a.Componente)
                .WithMany(c => c.Actividades)
                .HasForeignKey(a => a.ComponenteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_actividades_componentes");
        });

        // ===== TABLA PRODUCTOS_ESPERADOS =====
        modelBuilder.Entity<ProductoEsperado>(entity =>
        {
            entity.ToTable("productos_esperados", "public");
            entity.HasKey(pe => pe.Id);

            entity.Property(pe => pe.ComponenteId).HasColumnName("componente_id").IsRequired();
            entity.Property(pe => pe.Descripcion).IsRequired();

            // Relación con Componente
            entity.HasOne(pe => pe.Componente)
                .WithMany(c => c.ProductosEsperados)
                .HasForeignKey(pe => pe.ComponenteId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_productos_componentes");
        });

        // ===== TABLA OBSERVACIONES_CPGIC =====
        modelBuilder.Entity<ObservacionesCpgic>(entity =>
        {
            entity.ToTable("observaciones_cpgic", "public");
            entity.HasKey(o => o.Id);

            entity.Property(o => o.PropuestaId).IsRequired();
            entity.Property(o => o.Observacion).IsRequired();
            entity.Property(o => o.RealizadoPor).HasMaxLength(150);
            entity.Property(o => o.FechaObservacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Relación con Propuesta
            entity.HasOne(o => o.Propuesta)
                .WithMany(p => p.Observaciones)
                .HasForeignKey(o => o.PropuestaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_observaciones_propuestas");
        });

        // ===== TABLA APROBACIONES_CPGIC =====
        modelBuilder.Entity<AprobacionesCpgic>(entity =>
        {
            entity.ToTable("aprobaciones_cpgic", "public");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.PropuestaId).IsRequired();
            entity.Property(a => a.Resolucion).HasMaxLength(int.MaxValue);
            entity.Property(a => a.PresidenteCpgic)
                .HasColumnName("miembro_cpgic")
                .HasMaxLength(150);
            entity.Ignore(a => a.FirmaDirector);
            entity.Ignore(a => a.FirmaPresidente);
            entity.Property(a => a.EstadoAprobacion).HasMaxLength(50);

            // Relación con Propuesta (1:1)
            entity.HasOne(a => a.Propuesta)
                .WithOne(p => p.AprobacionCpgic)
                .HasForeignKey<AprobacionesCpgic>(a => a.PropuestaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_aprobaciones_propuestas");

            entity.HasIndex(a => a.PropuestaId).IsUnique();
        });
    }

    /// <summary>
    /// Convierte una cadena de PascalCase a snake_case
    /// Ejemplo: "PropuestaId" -> "propuesta_id"
    /// </summary>
    private static string ConvertToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var builder = new System.Text.StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsUpper(c) && i > 0)
            {
                builder.Append('_');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(char.ToLowerInvariant(c));
            }
        }

        return builder.ToString();
    }
}
