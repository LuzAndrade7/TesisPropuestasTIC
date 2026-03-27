# TesisTIC - Sistema de Gestión de Propuestas de Trabajos de Integración Curricular

## Estado del Proyecto

✅ **Rama ModuloA: Estructura Completa Implementada**

- Backend .NET 8 con arquitectura en capas ✅
- Frontend Angular 17 con componentes profesionales ✅
- Base de datos PostgreSQL 16 ✅
- 80+ archivos generados (4170+ líneas de código) ✅
- Documentación técnica completa ✅

## Descripción

Sistema web profesional para gestionar propuestas de **Trabajos de Integración Curricular (TIC)**. Permite registrar propuestas, enviarlas para revisión y asignar estudiantes.

## Stack Tecnológico

- **Backend:** .NET 8, ASP.NET Core, EF Core, PostgreSQL
- **Frontend:** Angular 17, Bootstrap 5, RxJS
- **Infraestructura:** Docker, docker-compose, Git

## Inicio Rápido

### Con Docker
```bash
git checkout ModuloA
docker-compose up
# Acceso: http://localhost:4200
```

### Manual
```bash
# Terminal 1: Base de datos
psql -U postgres < database/init.sql

# Terminal 2: Backend
cd backend && dotnet run --project TesisTIC.API/TesisTIC.API.csproj

# Terminal 3: Frontend
cd frontend/tesis-tic-app && npm install && npm start
```

## Acceso

- Frontend: http://localhost:4200
- Backend API: http://localhost:5000/api
- Swagger: http://localhost:5000/swagger

## Documentación

- [RESUMEN_CREACION.md](RESUMEN_CREACION.md) - Visión general completa
- [DESARROLLO.md](DESARROLLO.md) - Documentación técnica detallada
- [INICIAL.md](INICIAL.md) - Guía de inicio rápido

## Estructura del Proyecto

```
backend/                 # API REST .NET 8
  ├── TesisTIC.API/
  ├── TesisTIC.Application/
  ├── TesisTIC.Domain/
  ├── TesisTIC.Infrastructure/
  └── TesisTIC.sln

frontend/                # Cliente Angular 17
  └── tesis-tic-app/

database/                # Scripts PostgreSQL
  └── init.sql

docs/                    # Documentación
```

## Endpoints Principales

### Propuestas
- POST /api/propuestas - Crear
- GET /api/propuestas - Listar
- GET /api/propuestas/{id} - Obtener
- PUT /api/propuestas/{id} - Actualizar
- DELETE /api/propuestas/{id} - Eliminar
- PATCH /api/propuestas/{id}/estado - Cambiar estado

### Maestros
- GET /api/estados - Estados
- GET /api/docentes - Docentes
- GET /api/asignaturas - Asignaturas

## Características

✅ Crear propuestas completas
✅ Listar con filtros por estado
✅ Ver detalles detallados
✅ Editar propuestas
✅ Cambiar estado
✅ Asignar estudiantes
✅ Validaciones robustas
✅ Interfaz responsiva

## Requisitos

- .NET 8 SDK
- Node.js 18+
- PostgreSQL 16
- Docker (opcional)

## Rama del Proyecto

**Rama Activa:** `ModuloA`

```bash
git checkout ModuloA
git status
```

---

**Proyecto de Tesis - Ingeniería de Software - Módulo Propuestas de TIC**
