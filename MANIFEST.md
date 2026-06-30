# Manifest de Entrega - TesisTIC

Fecha de actualizacion: junio 2026  
Estado: version final para rama `main`

## Estado General

La aplicacion queda integrada con backend .NET 8, frontend Angular 17 y base de datos PostgreSQL Neon. La version actual cubre el flujo completo de propuestas TIC desde borrador hasta revision, observaciones, aprobacion, asignacion de estudiantes y eliminacion controlada.

## Alcance Entregado

Backend:

- API REST para propuestas, docentes, asignaturas, estudiantes, observaciones y asignacion de estudiantes.
- Validaciones de negocio en servicios.
- Estados de propuesta controlados.
- Persistencia con Entity Framework Core y PostgreSQL.
- Conexion a Neon actualizada con `Channel Binding=Require`.

Frontend:

- Tablero principal con filtros y acciones por estado.
- Formulario de propuesta con datos generales, asignaturas, modulos, actividades, productos y estudiantes propuestos.
- Validacion visual de campos obligatorios.
- Modales de confirmacion, cancelacion, eliminacion y avisos.
- Modal para el maximo de 288 horas por modulo.
- Vista de detalle completo.
- Flujo de asignacion de estudiantes.

Base de datos:

- Scripts SQL por sprint en `database/`.
- Tablas para propuestas, componentes, actividades, productos esperados, observaciones, historial y estudiantes.

## Historias Cubiertas

| HU | Descripcion | Estado |
| --- | --- | --- |
| HU01 | Crear propuestas TIC | Completa |
| HU02 | Tablero y filtros | Completa |
| HU03 | Enviar a revision | Completa |
| HU04 | Observaciones CPGIC y reenvio | Completa |
| HU05 | Editar propuestas permitidas | Completa |
| HU06 | Detalle de propuesta | Completa |
| HU07 | Asignar estudiantes | Completa |
| HU08 | Eliminar solo borradores | Completa |

## Validaciones Finales Importantes

- Participantes: minimo 2, maximo 5.
- Modulos: debe existir un modulo por participante.
- Horas: cada modulo tiene maximo 288 horas en total.
- Horas excedidas: el frontend ajusta el valor al cupo disponible y muestra un modal.
- Envio a revision: exige formulario completo y modulos validos.
- Edicion: permitida solo en BORRADOR u OBSERVADA.
- Eliminacion: permitida solo en BORRADOR.
- Asignacion: requiere correspondencia entre estudiantes, participantes y modulos.

## Archivos Clave del Producto

Backend:

- `backend/TesisTIC.API/Program.cs`
- `backend/TesisTIC.API/appsettings.json`
- `backend/TesisTIC.API/appsettings.Development.json`
- `backend/TesisTIC.API/Controllers/PropuestasController.cs`
- `backend/TesisTIC.API/Controllers/ObservacionesController.cs`
- `backend/TesisTIC.API/Controllers/EstudiantesController.cs`
- `backend/TesisTIC.Application/Services/PropuestaService.cs`
- `backend/TesisTIC.Application/Services/PropuestaEstudianteService.cs`
- `backend/TesisTIC.Infrastructure/Data/TesisTicDbContext.cs`
- `backend/TesisTIC.Infrastructure/Repositories/`
- `backend/TesisTIC.Domain/Entities/`
- `backend/TesisTIC.Application/DTOs/`

Frontend:

- `frontend/src/app/app.routes.ts`
- `frontend/src/app/services/propuesta.service.ts`
- `frontend/src/app/models/propuesta.model.ts`
- `frontend/src/app/components/tablero/`
- `frontend/src/app/components/formulario-propuesta/`
- `frontend/src/app/components/detalle-propuesta/`
- `frontend/src/app/components/asignar-estudiantes/`
- `frontend/src/environments/`

Base de datos:

- `database/sprint1_init.sql`
- `database/sprint2_observaciones.sql`
- `database/sprint3_historial_estados.sql`
- `database/sprint4_propuesta_estudiantes.sql`
- `database/Setup_Propuesta_Observada.sql`

Documentacion:

- `README.md`
- `MANIFEST.md`
- `documentation/README.md`
- `documentation/GUIA_EJECUCION.md`
- `documentation/RESUMEN_FINAL_COMPLETO.md`
- `documentation/RESUMEN_SESION.md`
- `documentation/guias/TESTING.md`
- `documentation/endpoints/README.md`
- `postman/TesisTIC_Collection.json`

## Verificacion

Backend:

```bash
cd backend
dotnet build TesisTIC.sln
```

Frontend:

```bash
cd frontend
npm run build
```

Resultado esperado:

- Compilacion correcta.
- Sin errores bloqueantes.
- Puede haber advertencias de paquetes o presupuesto de bundle.

## Recomendacion Para Main

Para llevar la version final a `main`, incluir codigo fuente backend, frontend, scripts de base de datos, documentacion actualizada y coleccion Postman. No incluir carpetas generadas como `bin/`, `obj/`, `node_modules/` o `dist/`.
