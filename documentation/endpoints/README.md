# Documentación de Endpoints API

## Resumen Ejecutivo

El sistema TesisTIC expone una API RESTful completa con **42+ endpoints** organizados en 8 historias de usuario (HU01-HU08).

**Base URL**: `http://localhost:5000/api`  
**Documentación Swagger**: `http://localhost:5000/swagger`  
**Content-Type**: `application/json`

---

## Guía de Lectura de Endpoints

Cada endpoint documentado incluye:

```
### [Descripción breve]
- **HU**: Historia de Usuario asociada
- **Método HTTP**: GET, POST, PUT, DELETE
- **Endpoint**: /ruta/{parámetros}
- **Autenticación**: Sí/No (planeada)
- **Request Body**: JSON si aplica
- **Response Success**: Código HTTP + ejemplo JSON
- **Response Error**: Códigos y mensajes de error
- **Validaciones**: Reglas de negocio aplicadas
- **Notas**: Consideraciones especiales
```

---

## Endpoints Globales (Transversales)

### Manejo de Errores (Todos los Endpoints)

Todos los endpoints retornan uno de estos códigos:

| Código                        | Significado                             | Ejemplo                                        |
| ----------------------------- | --------------------------------------- | ---------------------------------------------- |
| **200 OK**                    | Éxito                                   | `{ id: 1, titulo: "...", estado: "BORRADOR" }` |
| **201 Created**               | Recurso creado                          | Response con Location header                   |
| **204 No Content**            | Éxito sin body                          | Usado en algunos DELETE                        |
| **400 Bad Request**           | Error validación                        | `{ message: "ID inválido" }`                   |
| **404 Not Found**             | Recurso no existe                       | `{ message: "Propuesta no encontrada" }`       |
| **403 Forbidden**             | Operación no permitida en estado actual | `{ message: "Solo BORRADOR puede editarse" }`  |
| **500 Internal Server Error** | Error servidor                          | `{ message: "Error interno del servidor" }`    |

---

## Índice por Historia de Usuario

### [HU01 - Registro de Propuestas](./HU01-registro.md)

- POST /propuestas (Crear)
- GET /propuestas (Listar)
- GET /propuestas/{id} (Detalle)

**Endpoints Relacionados**:

- GET /asignaturas (Selector)
- GET /docentes (Selector)

### [HU02 - Tablero](./HU02-tablero.md)

- GET /propuestas (Con filtros)
- GET /propuestas?estado=BORRADOR

### [HU03 - Envío a Revisión](./HU03-revision.md)

- POST /propuestas/{id}/enviar-revision

### [HU04 - Observaciones CPGIC](./HU04-observaciones.md)

- POST /observaciones (Crear)
- GET /observaciones/propuesta/{propuestaId} (Listar)
- PUT /propuestas/{id}/cambiar-estado (PENDIENTE → OBSERVADA)
- POST /propuestas/{id}/reenviar-despues-observaciones (OBSERVADA → PENDIENTE)

### [HU05 - Edición de Propuestas](./HU05-edicion.md)

- PUT /propuestas/{id} (Actualizar)
- DELETE /propuestas/{id} (Eliminar - solo BORRADOR)

### [HU06 - Detalle Completo](./HU06-detalle.md)

- GET /propuestas/{id}/detalle (Vista completa con todo)

### [HU07 - Asignación de Estudiantes](./HU07-asignacion.md)

- GET /estudiantes (Todos)
- GET /estudiantes/buscar?searchTerm= (Buscar)
- GET /estudiantes/disponibles/{propuestaId} (No asignados)
- GET /estudiantes/asignados/{propuestaId} (Ya asignados)
- POST /estudiantes/{propuestaId}/asignar (Asignar 1-5 estudiantes)
- POST /propuestas/{id}/solicitar-nueva-aprobacion (APROBADA → PENDIENTE con motivo)

### [HU08 - Eliminación de Propuestas](./HU08-eliminacion.md)

- DELETE /propuestas/{id} (Solo BORRADOR)

---

## Convenciones

### Parámetros de URL

- `{id}` - ID numérico (entero positivo)
- `{propuestaId}` - ID de propuesta
- `?estado=BORRADOR` - Query string para filtros

### Formatos

- **Fechas**: ISO 8601 UTC (`2026-05-25T18:30:00Z`)
- **IDs**: Enteros positivos (`1`, `42`, etc)
- **Strings**: UTF-8 (soporta acentos, ñ, etc)
- **Booleanos**: `true`/`false` (JSON)

### Respuestas Paginadas (Cuando aplique)

```json
{
  "items": [ { ... }, { ... } ],
  "total": 42,
  "skip": 0,
  "take": 10
}
```

---

## Operaciones Frecuentes

### Crear y Editar Propuesta

```
1. POST /propuestas (HU01 T01-T05)
   Crea con estado BORRADOR

2. PUT /propuestas/{id} (HU05 T16-T17)
   Solo si estado es BORRADOR u OBSERVADA

3. POST /propuestas/{id}/enviar-revision (HU03 T10)
   BORRADOR → PENDIENTE (no editable)
```

### Flujo Aprobación

```
1. GET /propuestas (HU02)
   Tablero ve propuestas en PENDIENTE

2. GET /propuestas/{id}/detalle (HU06)
   CPGIC revisa detalle completo

3. POST /observaciones (HU04 T12-T13)
   Si hay problemas: PENDIENTE → OBSERVADA

4. GET /observaciones/propuesta/{propuestaId} (HU04 T14)
   Profesor ve observaciones

5. PUT /propuestas/{id} (HU05)
   Profesor corrige (estado OBSERVADA)

6. POST /propuestas/{id}/reenviar-despues-observaciones (HU04 T15)
   Reenviar correcciones: OBSERVADA → PENDIENTE

   O

   PUT /propuestas/{id}/cambiar-estado
   Si aprobada: PENDIENTE → APROBADA
```

### Flujo Asignación Estudiantes

```
1. GET /propuestas/{id}/detalle (HU06)
   Solo APROBADA o PENDIENTE permiten asignación

2. GET /estudiantes/disponibles/{propuestaId} (HU07 T22)
   Frontend obtiene lista desplegable

3. POST /estudiantes/{propuestaId}/asignar (HU07 T20)
   Asigna máximo 5, con motivo requerido

4. Si propuesta estaba APROBADA:
   POST /propuestas/{id}/solicitar-nueva-aprobacion (HU07 T25)
   Automático: APROBADA → PENDIENTE
```

### Eliminación

```
1. GET /propuestas/{id}/detalle (HU06)
   Valida estado: SOLO BORRADOR puede eliminarse

2. DELETE /propuestas/{id} (HU08 T26-T28)
   - 200 OK: Eliminada
   - 403 Forbidden: No es BORRADOR
   - 404 Not Found: No existe
```

---

## Headers Estándar

```http
Content-Type: application/json
Accept: application/json
User-Agent: (El del cliente)

Respuesta incluye:
Location: /api/propuestas/{id}  (solo en 201 Created)
```

---

## Ejemplos de Errores Comunes

### Error 400 - Bad Request

```json
// Causa: ID inválido
{
  "message": "ID inválido",
  "statusCode": 400
}

// Causa: Máximo de estudiantes excedido
{
  "message": "No se pueden asignar más de 5 estudiantes",
  "statusCode": 400
}
```

### Error 403 - Forbidden

```json
// Causa: Estado no permite operación
{
  "message": "No se puede eliminar propuesta en estado APROBADA. Solo se pueden eliminar propuestas en estado BORRADOR.",
  "statusCode": 403
}
```

### Error 404 - Not Found

```json
{
  "message": "Propuesta con ID 9999 no encontrada",
  "statusCode": 404
}
```

---

## Rendimiento y Límites

| Parámetro                              | Límite | Nota                   |
| -------------------------------------- | ------ | ---------------------- |
| **Máximo estudiantes por propuesta**   | 5      | Validado en backend    |
| **Máximo observaciones por propuesta** | ∞      | Sin límite documentado |
| **Máximo asignaturas por propuesta**   | ∞      | Sin límite documentado |
| **Timeout request**                    | 30s    | Estándar ASP.NET Core  |
| **Máximo body size**                   | 30MB   | Por defecto Kestrel    |

---

## Versionamiento (Futuro)

Actualmente: Versión única (v1 implícita)

Cuando se implemente versionamiento:

```
GET /api/v1/propuestas
GET /api/v2/propuestas  (con cambios)
```

---

## Testing

Para probar estos endpoints:

1. **Postman**: Ver colección en `/postman/TesisTIC_Collection.json`
2. **Swagger UI**: `http://localhost:5000/swagger/index.html`
3. **cURL**:
   ```bash
   curl -X POST http://localhost:5000/api/propuestas \
     -H "Content-Type: application/json" \
     -d '{"titulo":"Test",...}'
   ```
4. **Frontend Angular**: Servicio en `/frontend/src/app/services/propuesta.service.ts`

---

**Próximo**: Consulta la documentación específica de cada HU para detalles completos.
