# Documentacion Tecnica - TesisTIC

Este directorio contiene la documentacion tecnica y funcional de la version final del sistema TesisTIC.

## Descripcion

TesisTIC gestiona propuestas de trabajos de titulacion TIC. El sistema cubre el registro por parte del profesor, revision CPGIC, observaciones, correccion, aprobacion, asignacion de estudiantes y eliminacion controlada.

## Indice Recomendado

- `../README.md`: resumen del proyecto y ejecucion rapida.
- `../MANIFEST.md`: manifest de entrega y archivos importantes.
- `GUIA_EJECUCION.md`: guia para levantar backend y frontend.
- `RESUMEN_FINAL_COMPLETO.md`: resumen completo de la version final.
- `RESUMEN_SESION.md`: cambios recientes consolidados.
- `arquitectura/README.md`: arquitectura del sistema.
- `endpoints/README.md`: endpoints REST.
- `base-datos/README.md`: modelo de base de datos.
- `flujos/README.md`: flujos de estados y operaciones.
- `seguridad/README.md`: validaciones y consideraciones de seguridad.
- `guias/TESTING.md`: guia de pruebas.

## Stack Tecnico

Backend:

- ASP.NET Core 8
- Entity Framework Core 8
- PostgreSQL Neon
- Npgsql
- AutoMapper
- Swagger

Frontend:

- Angular 17 standalone
- TypeScript
- SCSS
- RxJS

## Rutas Principales

Frontend:

- `/tablero`
- `/propuestas/nueva`
- `/propuestas/:id`
- `/propuestas/:id/editar`
- `/propuestas/:id/detalle`
- `/propuestas/:id/asignar-estudiantes`

API principal:

- `GET /api/propuestas`
- `GET /api/propuestas/{id}`
- `GET /api/propuestas/{id}/detalle`
- `POST /api/propuestas`
- `PUT /api/propuestas/{id}`
- `DELETE /api/propuestas/{id}`
- `POST /api/propuestas/{id}/enviar-revision`
- `POST /api/propuestas/{id}/reenviar-despues-observaciones`
- `POST /api/propuestas/{id}/solicitar-nueva-aprobacion`
- `GET /api/observaciones/propuesta/{id}`
- `POST /api/observaciones`
- `GET /api/estudiantes`
- `POST /api/estudiantes`

## Estados de Propuesta

```text
BORRADOR
  -> PENDIENTE
  -> OBSERVADA
  -> PENDIENTE
  -> APROBADA
  -> PENDIENTE si se solicita nueva aprobacion
```

Reglas:

- BORRADOR: editable y eliminable.
- OBSERVADA: editable para correccion y reenviable.
- PENDIENTE: no editable por profesor.
- APROBADA: permite asignacion de estudiantes; cambios pueden solicitar nueva aprobacion.
- RECHAZADA: no editable.

## Validaciones Finales

- Minimo 2 y maximo 5 participantes.
- La cantidad de modulos debe coincidir con participantes.
- Cada modulo debe tener nombre, descripcion, actividades y productos esperados para enviar.
- Cada modulo tiene maximo 288 horas.
- El formulario muestra un modal si se intenta superar 288 horas.
- La asignacion de estudiantes exige entre 2 y 5 estudiantes y uno por modulo.
- La eliminacion solo procede en BORRADOR.

## Notas de Calidad

La version actual compila en backend y frontend. El build de Angular puede mostrar advertencias de tamano, pero no errores. La conexion a Neon se configura fuera de Git mediante variable de entorno, user-secrets o `appsettings.Local.json`.
