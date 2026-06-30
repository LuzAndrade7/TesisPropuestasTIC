# TesisTIC - Sistema de Gestion de Propuestas TIC

Sistema web para registrar, revisar, corregir, aprobar y administrar propuestas de trabajos de titulacion TIC.

Version final local: junio 2026  
Estado: listo para rama principal

## Resumen

El sistema permite que un profesor cree propuestas, las guarde como borrador, las envie a revision, reciba observaciones de CPGIC, corrija y reenvie. Tambien permite consultar el detalle completo, asignar estudiantes a propuestas aprobadas, solicitar nueva aprobacion cuando cambia la asignacion y eliminar solo propuestas en estado BORRADOR.

## Stack

Backend:

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL Neon
- Npgsql
- AutoMapper
- Swagger/OpenAPI

Frontend:

- Angular 17 standalone
- TypeScript
- SCSS
- RxJS
- Angular Router

Base de datos:

- PostgreSQL en Neon
- Scripts SQL por sprint en `database/`

## Funcionalidad Final

Historias implementadas:

- HU01: Registro de propuestas en estado BORRADOR.
- HU02: Tablero con listado, filtros por estado y acciones disponibles.
- HU03: Envio a revision con validaciones.
- HU04: Observaciones CPGIC y reenvio de propuestas observadas.
- HU05: Edicion de propuestas en BORRADOR u OBSERVADA.
- HU06: Detalle completo de propuesta.
- HU07: Asignacion de estudiantes por modulo.
- HU08: Eliminacion restringida a propuestas en BORRADOR.

Validaciones principales:

- Solo se editan propuestas en BORRADOR u OBSERVADA.
- Solo se eliminan propuestas en BORRADOR.
- El numero de participantes permitido es de 2 a 5.
- Cada propuesta debe tener un modulo por participante.
- Cada modulo permite actividades, productos y estudiante propuesto.
- El total de horas por modulo no puede superar 288.
- Si se intenta superar 288 horas, el formulario ajusta al cupo restante y muestra un modal informativo.
- La asignacion de estudiantes exige correspondencia entre participantes, modulos y estudiantes.

## Estructura

```text
TesisPropuestasTIC-1/
  backend/
    TesisTIC.sln
    TesisTIC.API/
    TesisTIC.Application/
    TesisTIC.Domain/
    TesisTIC.Infrastructure/
  frontend/
    src/app/components/
    src/app/services/
    src/app/models/
  database/
  documentation/
  postman/
```

## Ejecucion

Backend:

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project TesisTIC.API
```

Frontend:

```bash
cd frontend
npm install
npm start
```

URLs locales:

- Frontend: `http://localhost:4200`
- Swagger: `http://localhost:5000/swagger`
- API: `http://localhost:5000/api`

## Verificacion Reciente

Comandos ejecutados sobre la version final:

```bash
cd backend
dotnet build TesisTIC.sln

cd frontend
npm run build
```

Resultado:

- Backend compila sin errores.
- Frontend compila sin errores.
- El build de frontend muestra advertencias de presupuesto de tamano, pero no bloquean la compilacion.
- La conexion a Neon fue validada con Npgsql usando la cadena configurada.

## Documentacion

Documentos principales:

- `MANIFEST.md`: estado de entrega y archivos importantes.
- `documentation/README.md`: indice tecnico.
- `documentation/GUIA_EJECUCION.md`: pasos para ejecutar.

## Configuracion Segura

La cadena real de Neon no debe guardarse en Git. Los `appsettings` versionados dejan `TesisTICConnection` vacia y el backend la toma desde:

- Variable de entorno `ConnectionStrings__TesisTICConnection`.
- `dotnet user-secrets`.
- `backend/TesisTIC.API/appsettings.Local.json`, que esta ignorado por Git.

Hay una plantilla en `backend/TesisTIC.API/appsettings.Local.example.json`.
