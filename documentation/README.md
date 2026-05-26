# TesisTIC - Documentación Técnica Completa

## Descripción General

Sistema de gestión de propuestas TIC para la Escuela Politécnica Nacional (EPN), permitiendo que docentes propongan proyectos, que sean revisados por la comisión CPGIC, y que se asignen estudiantes a proyectos aprobados.

**Versión**: 1.0  
**Último Actualizado**: Mayo 2026  
**Estado**: Producción

---

## Índice de Documentación

### 1. [Arquitectura del Sistema](./arquitectura/README.md)

- Estructura capas (Presentation, Application, Domain, Infrastructure)
- Tecnologías usadas
- Patrones de diseño
- Diagrama textual de arquitectura

### 2. [Endpoints API](./endpoints/README.md)

- Lista completa de endpoints por historia de usuario
- Métodos HTTP, URLs, parámetros
- Request/Response examples
- Códigos HTTP y errores

**Endpoints Específicos:**

- [HU01 - Registro de Propuestas](./endpoints/HU01-registro.md)
- [HU02 - Tablero](./endpoints/HU02-tablero.md)
- [HU03 - Envío a Revisión](./endpoints/HU03-revision.md)
- [HU04 - Observaciones](./endpoints/HU04-observaciones.md)
- [HU05 - Edición](./endpoints/HU05-edicion.md)
- [HU06 - Detalle Propuesta](./endpoints/HU06-detalle.md)
- [HU07 - Asignación Estudiantes](./endpoints/HU07-asignacion.md)
- [HU08 - Eliminación](./endpoints/HU08-eliminacion.md)

### 3. [Base de Datos](./base-datos/README.md)

- Diagrama ER (relaciones)
- Descripción de tablas
- Foreign keys y constraints
- Índices y optimizaciones
- Scripts de inicialización

### 4. [Flujos del Sistema](./flujos/README.md)

- Flujo de estados de propuestas (BORRADOR → PENDIENTE → ... → APROBADA)
- Diagramas de secuencia
- Casos de uso
- Decisiones de diseño

### 5. [Seguridad](./seguridad/README.md)

- Endpoints protegidos vs públicos
- Roles y permisos (en desarrollo)
- Validaciones de entrada
- Manejo de errores

### 6. [Guías de Pruebas](./guias/README.md)

- Orden recomendado de pruebas
- Casos de prueba por HU
- Datos de prueba
- Checklist de validación
- Colección Postman

### 7. [Relaciones Base de Datos](./base-datos/relaciones.md)

- Explicación detallada de relaciones
- Cascadas de eliminación
- Integridad referencial
- Ejemplos de queries

---

## Stack Tecnológico

### Backend

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL 16 (Neon)
- **Autenticación**: (Planeada)
- **API Docs**: Swagger/OpenAPI 6.4.6
- **Mapper**: AutoMapper 13.0.1

### Frontend

- **Framework**: Angular 17 (Standalone)
- **Lenguaje**: TypeScript 5.x
- **Styling**: SCSS + Sistema de colores EPN
- **HTTP Client**: RxJS 7.8.0
- **Routing**: Angular Router

### DevOps

- **Contenedor**: Docker
- **Orquestación**: Docker Compose
- **Control de Versiones**: Git

---

## URLs Principales

| Ambiente             | URL                                                              | Estado        |
| -------------------- | ---------------------------------------------------------------- | ------------- |
| **Frontend (Local)** | `http://localhost:4200`                                          | ✅ Desarrollo |
| **Backend (Local)**  | `http://localhost:5000`                                          | ✅ Desarrollo |
| **API Swagger**      | `http://localhost:5000/swagger`                                  | ✅ Disponible |
| **BD (Neon)**        | `ep-lingering-mouse-aqf2l271-pooler.c-8.us-east-1.aws.neon.tech` | ✅ Producción |

---

## Historias de Usuario Implementadas

### ✅ HU01: Registro de Propuestas (T01-T05)

- Crear propuesta en estado BORRADOR
- Rellenar datos básicos, objetivo, alcance
- Asignar asignaturas y profesor

### ✅ HU02: Tablero (T06-T09)

- Listar propuestas con filtros por estado
- Resumen visual de estados
- Acciones contextuales por estado

### ✅ HU03: Envío a Revisión (T10)

- BORRADOR → PENDIENTE
- Validación de campos requeridos
- Bloqueo de edición post-envío

### ✅ HU04: Observaciones CPGIC (T11-T15)

- Asignar observaciones a propuestas PENDIENTE
- PENDIENTE → OBSERVADA
- Reenvío con correcciones (OBSERVADA → PENDIENTE)

### ✅ HU05: Edición de Propuestas (T16-T17)

- Editar solo BORRADOR y OBSERVADA
- Cambiar asignaturas
- Validación de cambios

### ✅ HU06: Detalle Completo (T18-T19)

- Vista completa de propuesta
- Información básica, profesor, asignaturas
- Observaciones e histórico de estados

### ✅ HU07: Asignación de Estudiantes (T20-T25)

- Asignar máximo 5 estudiantes a propuestas APROBADA/PENDIENTE
- Si cambios en APROBADA → automático PENDIENTE
- Solicitar nueva aprobación con motivo

### ✅ HU08: Eliminación de Propuestas (T26-T28)

- Eliminar SOLO propuestas en BORRADOR
- Cascada elimina: asignaturas, estudiantes, observaciones, histórico
- Confirmación y redirección automática

---

## Flujo de Estados Principales

```
BORRADOR (Editable)
    ↓
    [Enviar a Revisión]
    ↓
PENDIENTE (No editable, esperando CPGIC)
    ↓
    ┌──────────────────────────────────────┐
    ↓                                      ↓
[Si hay problemas]              [Si está bien]
    ↓                                      ↓
OBSERVADA (Puede reenviarse)       APROBADA
    ↑_____________↓                        ↓
                 [Reenvío]        [Asignar estudiantes]
                                           ↓
                                  [Si cambios]
                                    PENDIENTE (vuelve)
```

**Estados Bloqueados para Eliminación**: PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
**Solo BORRADOR puede eliminarse**

---

## Validaciones Globales

| Validación                    | Lugar               | Efecto          |
| ----------------------------- | ------------------- | --------------- |
| Estado = BORRADOR para editar | Servicio            | 400 Bad Request |
| Max 5 estudiantes             | Servicio + Frontend | 400 Bad Request |
| Motivo requerido si cambios   | Frontend            | UI block        |
| BORRADOR solo para eliminar   | Servicio            | 403 Forbidden   |
| Propuesta existe              | Repositorio         | 404 Not Found   |
| ID válido (> 0)               | Servicio            | 400 Bad Request |

---

## Configuración Inicial

### Variables de Entorno (Backend)

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionString__TesisTIC=Host=ep-lingering-mouse-aqf2l271-pooler.c-8.us-east-1.aws.neon.tech;Database=testistic;...
```

### Variables de Entorno (Frontend)

```typescript
// environments/environment.ts
export const environment = {
  apiUrl: "http://localhost:5000/api",
};
```

---

## Próximas Iteraciones (Roadmap)

- [ ] Autenticación con OAuth2
- [ ] Roles y permisos (Profesor, CPGIC, Admin)
- [ ] Notificaciones por email
- [ ] Exportar propuestas a PDF
- [ ] Dashboard analítico
- [ ] Auditoría completa (logs)
- [ ] Tests automatizados

---

## Contacto y Soporte

**Desarrollador**: [Tu Nombre]  
**Email**: [email]  
**Fecha Última Revisión**: Mayo 25, 2026  
**Versión Documento**: 1.0

---

**Nota**: Esta documentación se mantiene actualizada con cada release. Consulta la carpeta `/documentation` para detalles específicos.
