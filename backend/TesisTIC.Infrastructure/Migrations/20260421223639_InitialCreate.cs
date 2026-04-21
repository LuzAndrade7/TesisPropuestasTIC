using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TesisTIC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asignaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignaturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CorreoInstitucional = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CorreoPersonal = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Departamento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaAtualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Matricula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CorreoInstitucional = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineasInvestigacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasInvestigacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Proyecto = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Objetivo = table.Column<string>(type: "text", nullable: false),
                    Alcance = table.Column<string>(type: "text", nullable: false),
                    NumeroParticipantes = table.Column<int>(type: "integer", nullable: false),
                    Departamento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Facultad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DocenteId = table.Column<int>(type: "integer", nullable: false),
                    EstadoId = table.Column<int>(type: "integer", nullable: false),
                    LineaInvestigacionId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaEnvioPrimera = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Propuestas_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Propuestas_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Propuestas_LineasInvestigacion_LineaInvestigacionId",
                        column: x => x.LineaInvestigacionId,
                        principalTable: "LineasInvestigacion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    NumeroModulo = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Productos = table.Column<string>(type: "text", nullable: false),
                    EstudianteAsignado = table.Column<string>(type: "text", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modulos_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservacionesPropuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservacionesPropuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservacionesPropuestas_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropuestasAsignaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    AsignaturaId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasAsignaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestasAsignaturas_Asignaturas_AsignaturaId",
                        column: x => x.AsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropuestasAsignaturas_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropuestasEstudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    EstudianteId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasEstudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestasEstudiantes_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropuestasEstudiantes_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RechazoPropuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropuestaId = table.Column<int>(type: "integer", nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Recomendacion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RechazoPropuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RechazoPropuestas_Propuestas_PropuestaId",
                        column: x => x.PropuestaId,
                        principalTable: "Propuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuloId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Horas = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actividades_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservacionesModulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuloId = table.Column<int>(type: "integer", nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservacionesModulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservacionesModulos_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RechazoRazones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RechazoPropuestaId = table.Column<int>(type: "integer", nullable: false),
                    Razon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RechazoRazones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RechazoRazones_RechazoPropuestas_RechazoPropuestaId",
                        column: x => x.RechazoPropuestaId,
                        principalTable: "RechazoPropuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Estados",
                columns: new[] { "Id", "Descripcion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Propuesta creada, pendiente de revisión", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1861), "Pendiente" },
                    { 2, "Propuesta enviada para revisión por CPGIC", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1865), "Enviada" },
                    { 3, "Propuesta bajo revisión", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1867), "En Revisión" },
                    { 4, "Propuesta aprobada", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1869), "Aprobada" },
                    { 5, "Propuesta rechazada", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1870), "Rechazada" },
                    { 6, "Propuesta con observaciones, requiere ajustes", new DateTime(2026, 4, 21, 22, 36, 38, 386, DateTimeKind.Utc).AddTicks(1871), "Observada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_ModuloId",
                table: "Actividades",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_PropuestaId",
                table: "Modulos",
                column: "PropuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservacionesModulos_ModuloId",
                table: "ObservacionesModulos",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservacionesPropuestas_PropuestaId",
                table: "ObservacionesPropuestas",
                column: "PropuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_DocenteId",
                table: "Propuestas",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_EstadoId",
                table: "Propuestas",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Propuestas_LineaInvestigacionId",
                table: "Propuestas",
                column: "LineaInvestigacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasAsignaturas_AsignaturaId",
                table: "PropuestasAsignaturas",
                column: "AsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasAsignaturas_PropuestaId_AsignaturaId",
                table: "PropuestasAsignaturas",
                columns: new[] { "PropuestaId", "AsignaturaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasEstudiantes_EstudianteId",
                table: "PropuestasEstudiantes",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasEstudiantes_PropuestaId_EstudianteId",
                table: "PropuestasEstudiantes",
                columns: new[] { "PropuestaId", "EstudianteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RechazoPropuestas_PropuestaId",
                table: "RechazoPropuestas",
                column: "PropuestaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RechazoRazones_RechazoPropuestaId",
                table: "RechazoRazones",
                column: "RechazoPropuestaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actividades");

            migrationBuilder.DropTable(
                name: "ObservacionesModulos");

            migrationBuilder.DropTable(
                name: "ObservacionesPropuestas");

            migrationBuilder.DropTable(
                name: "PropuestasAsignaturas");

            migrationBuilder.DropTable(
                name: "PropuestasEstudiantes");

            migrationBuilder.DropTable(
                name: "RechazoRazones");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Asignaturas");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "RechazoPropuestas");

            migrationBuilder.DropTable(
                name: "Propuestas");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "LineasInvestigacion");
        }
    }
}
