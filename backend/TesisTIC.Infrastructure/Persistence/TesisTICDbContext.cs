using Microsoft.EntityFrameworkCore;
using TesisTIC.Domain.Entities;

namespace TesisTIC.Infrastructure.Persistence
{
    public class TesisTICDbContext : DbContext
    {
        public TesisTICDbContext(DbContextOptions<TesisTICDbContext> options) : base(options)
        {
        }

        public DbSet<Propuesta> Propuestas { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<LineaInvestigacion> LineasInvestigacion { get; set; }
        public DbSet<Asignatura> Asignaturas { get; set; }
        public DbSet<PropuestaEstudiante> PropuestasEstudiantes { get; set; }
        public DbSet<PropuestaAsignatura> PropuestasAsignaturas { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<ObservacionPropuesta> ObservacionesPropuestas { get; set; }
        public DbSet<ObservacionModulo> ObservacionesModulos { get; set; }
        public DbSet<RechazoPropuesta> RechazoPropuestas { get; set; }
        public DbSet<RechazoRazon> RechazoRazones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasMany(e => e.Propuestas).WithOne(p => p.Estado).HasForeignKey(p => p.EstadoId);
            });

            modelBuilder.Entity<Docente>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(d => d.CorreoInstitucional).IsRequired().HasMaxLength(150);
                entity.Property(d => d.CorreoPersonal).HasMaxLength(150);
                entity.Property(d => d.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(d => d.NumeroEmpleado).HasMaxLength(20);
                entity.Property(d => d.Departamento).HasMaxLength(150);
                entity.HasMany(d => d.Propuestas).WithOne(p => p.Docente).HasForeignKey(p => p.DocenteId);
            });

            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Matricula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CorreoInstitucional).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.HasMany(e => e.PropuestasAsignadas).WithOne(pe => pe.Estudiante).HasForeignKey(pe => pe.EstudianteId);
            });

            modelBuilder.Entity<LineaInvestigacion>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(l => l.Descripcion).HasMaxLength(500);
                entity.HasMany(l => l.Propuestas).WithOne(p => p.LineaInvestigacion).HasForeignKey(p => p.LineaInvestigacionId);
            });

            modelBuilder.Entity<Asignatura>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Codigo).IsRequired().HasMaxLength(20);
                entity.HasMany(a => a.PropuestasAsignadas).WithOne(pa => pa.Asignatura).HasForeignKey(pa => pa.AsignaturaId);
            });

            modelBuilder.Entity<Propuesta>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Titulo).IsRequired().HasMaxLength(300);
                entity.Property(p => p.Descripcion).IsRequired();
                entity.Property(p => p.Objetivo).IsRequired();
                entity.Property(p => p.Alcance).IsRequired();
                entity.Property(p => p.Departamento).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Facultad).IsRequired().HasMaxLength(200);
                entity.HasMany(p => p.EstudiantesAsignados).WithOne(pe => pe.Propuesta).HasForeignKey(pe => pe.PropuestaId);
                entity.HasMany(p => p.AsignaturasAsignadas).WithOne(pa => pa.Propuesta).HasForeignKey(pa => pa.PropuestaId);
                entity.HasMany(p => p.Modulos).WithOne(m => m.Propuesta).HasForeignKey(m => m.PropuestaId);
                entity.HasMany(p => p.Observaciones).WithOne(o => o.Propuesta).HasForeignKey(o => o.PropuestaId);
                entity.HasOne(p => p.Rechazo).WithOne(r => r.Propuesta).HasForeignKey<RechazoPropuesta>(r => r.PropuestaId);
            });

            modelBuilder.Entity<PropuestaAsignatura>(entity =>
            {
                entity.HasKey(pa => pa.Id);
                entity.HasOne(pa => pa.Propuesta).WithMany(p => p.AsignaturasAsignadas).HasForeignKey(pa => pa.PropuestaId);
                entity.HasOne(pa => pa.Asignatura).WithMany(a => a.PropuestasAsignadas).HasForeignKey(pa => pa.AsignaturaId);
                entity.HasIndex(pa => new { pa.PropuestaId, pa.AsignaturaId }).IsUnique();
            });

            modelBuilder.Entity<PropuestaEstudiante>(entity =>
            {
                entity.HasKey(pe => pe.Id);
                entity.HasOne(pe => pe.Propuesta).WithMany(p => p.EstudiantesAsignados).HasForeignKey(pe => pe.PropuestaId);
                entity.HasOne(pe => pe.Estudiante).WithMany(e => e.PropuestasAsignadas).HasForeignKey(pe => pe.EstudianteId);
                entity.HasIndex(pe => new { pe.PropuestaId, pe.EstudianteId }).IsUnique();
            });

            modelBuilder.Entity<Modulo>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Descripcion).IsRequired();
                entity.Property(m => m.Productos).IsRequired();
                entity.HasOne(m => m.Propuesta).WithMany(p => p.Modulos).HasForeignKey(m => m.PropuestaId);
                entity.HasMany(m => m.Actividades).WithOne(a => a.Modulo).HasForeignKey(a => a.ModuloId);
                entity.HasMany(m => m.Observaciones).WithOne(o => o.Modulo).HasForeignKey(o => o.ModuloId);
            });

            modelBuilder.Entity<Actividad>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Descripcion).IsRequired().HasMaxLength(300);
                entity.HasOne(a => a.Modulo).WithMany(m => m.Actividades).HasForeignKey(a => a.ModuloId);
            });

            modelBuilder.Entity<ObservacionPropuesta>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Autor).IsRequired().HasMaxLength(200);
                entity.Property(o => o.Texto).IsRequired();
                entity.HasOne(o => o.Propuesta).WithMany(p => p.Observaciones).HasForeignKey(o => o.PropuestaId);
            });

            modelBuilder.Entity<ObservacionModulo>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Autor).IsRequired().HasMaxLength(200);
                entity.Property(o => o.Texto).IsRequired();
                entity.HasOne(o => o.Modulo).WithMany(m => m.Observaciones).HasForeignKey(o => o.ModuloId);
            });

            modelBuilder.Entity<RechazoPropuesta>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Autor).IsRequired().HasMaxLength(200);
                entity.Property(r => r.Recomendacion).IsRequired();
                entity.HasOne(r => r.Propuesta).WithOne(p => p.Rechazo).HasForeignKey<RechazoPropuesta>(r => r.PropuestaId);
                entity.HasMany(r => r.Razones).WithOne(rz => rz.RechazoPropuesta).HasForeignKey(rz => rz.RechazoPropuestaId);
            });

            modelBuilder.Entity<RechazoRazon>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Razon).IsRequired();
                entity.HasOne(r => r.RechazoPropuesta).WithMany(rp => rp.Razones).HasForeignKey(r => r.RechazoPropuestaId);
            });

            ConfiguracionesPredeterminadas(modelBuilder);
        }

        private void ConfiguracionesPredeterminadas(ModelBuilder modelBuilder)
        {
            var estados = new Estado[]
            {
                new Estado { Id = 1, Nombre = "Pendiente", Descripcion = "Propuesta creada, pendiente de revisión", FechaCreacion = DateTime.UtcNow },
                new Estado { Id = 2, Nombre = "Enviada", Descripcion = "Propuesta enviada para revisión por CPGIC", FechaCreacion = DateTime.UtcNow },
                new Estado { Id = 3, Nombre = "En Revisión", Descripcion = "Propuesta bajo revisión", FechaCreacion = DateTime.UtcNow },
                new Estado { Id = 4, Nombre = "Aprobada", Descripcion = "Propuesta aprobada", FechaCreacion = DateTime.UtcNow },
                new Estado { Id = 5, Nombre = "Rechazada", Descripcion = "Propuesta rechazada", FechaCreacion = DateTime.UtcNow },
                new Estado { Id = 6, Nombre = "Observada", Descripcion = "Propuesta con observaciones, requiere ajustes", FechaCreacion = DateTime.UtcNow }
            };

            modelBuilder.Entity<Estado>().HasData(estados);
        }
    }
}
