# 📘 GUÍA ENDPOINTS API TESTISTIC

## 🌐 URL BASE

```
http://localhost:5000/api
```

## 📋 ENDPOINTS DISPONIBLES

---

## 🎯 PROPUESTAS

### 1. Obtener todas las propuestas

```
GET /api/propuestas
```

**Response:**

```json
[
  {
    "id": 1,
    "nombreProyecto": "Sistema de Gestión Académica",
    "numeroParticipantes": 3,
    "profesorId": 1,
    "descripcion": "Descripción del proyecto",
    "objetivo": "Objetivo principal",
    "alcance": "Alcance del proyecto",
    "estado": "BORRADOR",
    "fechaCreacion": "2026-05-17T10:30:00Z",
    "fechaActualizacion": "2026-05-17T10:30:00Z",
    "fechaEnvioRevision": null,
    "profesor": {
      "id": 1,
      "nombres": "Juan",
      "apellidos": "Pérez",
      "correo": "juan.perez@epn.edu.ec",
      "tituloAcademico": "Ingeniero"
    },
    "asignaturas": [
      {
        "id": 1,
        "codigo": "ISTG2001",
        "nombre": "Ingeniería de Software"
      }
    ]
  }
]
```

---

### 2. Obtener propuesta por ID

```
GET /api/propuestas/{id}
```

**Ejemplo:**

```
GET /api/propuestas/1
```

---

### 3. Obtener propuestas por estado

```
GET /api/propuestas/por-estado/{estado}
```

**Estados válidos:** `BORRADOR`, `PENDIENTE`, `OBSERVADA`, `APROBADA`, `RECHAZADA`

**Ejemplo:**

```
GET /api/propuestas/por-estado/PENDIENTE
```

---

### 4. Obtener propuestas por profesor

```
GET /api/propuestas/profesor/{profesorId}
```

**Ejemplo:**

```
GET /api/propuestas/profesor/1
```

---

### 5. Crear propuesta

```
POST /api/propuestas
Content-Type: application/json
```

**Body:**

```json
{
  "nombreProyecto": "Sistema de Gestión de Propuestas",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Sistema para gestionar propuestas TIC de la EPN",
  "objetivo": "Desarrollar un sistema integral de gestión",
  "alcance": "Aplicable a toda la facultad",
  "asignaturaIds": [1, 2]
}
```

**Response (201 Created):**

```json
{
  "id": 5,
  "nombreProyecto": "Sistema de Gestión de Propuestas",
  "numeroParticipantes": 3,
  "profesorId": 1,
  "descripcion": "Sistema para gestionar propuestas TIC de la EPN",
  "objetivo": "Desarrollar un sistema integral de gestión",
  "alcance": "Aplicable a toda la facultad",
  "estado": "BORRADOR",
  "fechaCreacion": "2026-05-17T10:30:00Z",
  "fechaActualizacion": "2026-05-17T10:30:00Z",
  "fechaEnvioRevision": null,
  "profesor": null,
  "asignaturas": []
}
```

---

### 6. Actualizar propuesta

```
PUT /api/propuestas/{id}
Content-Type: application/json
```

**Body (todos los campos opcionales):**

```json
{
  "nombreProyecto": "Sistema mejorado",
  "numeroParticipantes": 4,
  "descripcion": "Descripción actualizada",
  "objetivo": "Nuevo objetivo",
  "alcance": "Nuevo alcance",
  "asignaturaIds": [1, 2, 3]
}
```

---

### 7. Cambiar estado propuesta

```
PATCH /api/propuestas/{id}/estado
Content-Type: application/json
```

**Body:**

```json
{
  "estado": "PENDIENTE"
}
```

**Estados válidos:** `BORRADOR`, `PENDIENTE`, `OBSERVADA`, `APROBADA`, `RECHAZADA`

---

### 8. Asignar asignatura a propuesta

```
POST /api/propuestas/{propuestaId}/asignaturas/{asignaturaId}
```

**Ejemplo:**

```
POST /api/propuestas/1/asignaturas/2
```

---

### 9. Remover asignatura de propuesta

```
DELETE /api/propuestas/{propuestaId}/asignaturas/{asignaturaId}
```

**Ejemplo:**

```
DELETE /api/propuestas/1/asignaturas/2
```

---

### 10. Eliminar propuesta

```
DELETE /api/propuestas/{id}
```

**Ejemplo:**

```
DELETE /api/propuestas/1
```

---

## 👨‍🏫 DOCENTES

### 1. Obtener todos los docentes

```
GET /api/docentes
```

---

### 2. Obtener docente por ID

```
GET /api/docentes/{id}
```

---

### 3. Crear docente

```
POST /api/docentes
Content-Type: application/json
```

**Body:**

```json
{
  "nombres": "Carlos",
  "apellidos": "González",
  "correo": "carlos.gonzalez@epn.edu.ec",
  "tituloAcademico": "Ingeniero en Sistemas"
}
```

---

### 4. Actualizar docente

```
PUT /api/docentes/{id}
Content-Type: application/json
```

**Body:**

```json
{
  "nombres": "Carlos",
  "apellidos": "González Martínez",
  "correo": "cgonzalez@epn.edu.ec",
  "tituloAcademico": "Doctor en Informática"
}
```

---

### 5. Eliminar docente

```
DELETE /api/docentes/{id}
```

---

## 📚 ASIGNATURAS

### 1. Obtener todas las asignaturas

```
GET /api/asignaturas
```

---

### 2. Obtener asignatura por ID

```
GET /api/asignaturas/{id}
```

---

### 3. Crear asignatura

```
POST /api/asignaturas
Content-Type: application/json
```

**Body:**

```json
{
  "codigo": "ISTG2001",
  "nombre": "Ingeniería de Software I"
}
```

---

### 4. Actualizar asignatura

```
PUT /api/asignaturas/{id}
Content-Type: application/json
```

**Body:**

```json
{
  "codigo": "ISTG2001",
  "nombre": "Ingeniería de Software I - Actualizado"
}
```

---

### 5. Eliminar asignatura

```
DELETE /api/asignaturas/{id}
```

---

## 👤 ESTUDIANTES

### 1. Obtener todos los estudiantes

```
GET /api/estudiantes
```

---

### 2. Obtener estudiante por ID

```
GET /api/estudiantes/{id}
```

---

### 3. Crear estudiante

```
POST /api/estudiantes
Content-Type: application/json
```

**Body:**

```json
{
  "nombres": "María",
  "apellidos": "López",
  "correo": "maria.lopez@epn.edu.ec"
}
```

---

### 4. Actualizar estudiante

```
PUT /api/estudiantes/{id}
Content-Type: application/json
```

**Body:**

```json
{
  "nombres": "María",
  "apellidos": "López García",
  "correo": "mlopez@epn.edu.ec"
}
```

---

### 5. Eliminar estudiante

```
DELETE /api/estudiantes/{id}
```

---

## 🔍 CÓDIGOS DE RESPUESTA HTTP

| Código | Significado                                 |
| ------ | ------------------------------------------- |
| 200    | OK - Operación exitosa                      |
| 201    | Created - Recurso creado exitosamente       |
| 400    | Bad Request - Error en los parámetros/datos |
| 404    | Not Found - Recurso no encontrado           |
| 500    | Internal Server Error - Error del servidor  |

---

## 📍 SWAGGER UI

Accede a la documentación interactiva en:

```
http://localhost:5000/swagger
```

---

## 🏥 HEALTH CHECK

Verifica el estado de la API:

```
GET /health
```

**Response:**

```json
{
  "status": "healthy",
  "timestamp": "2026-05-17T10:30:00Z"
}
```

---

## 💾 EJEMPLO FLUJO COMPLETO

### 1. Crear un docente

```bash
curl -X POST "http://localhost:5000/api/docentes" \
  -H "Content-Type: application/json" \
  -d '{
    "nombres": "Juan",
    "apellidos": "Pérez",
    "correo": "juan@epn.edu.ec",
    "tituloAcademico": "Ingeniero"
  }'
```

**Response:** `{"id": 1, ...}`

### 2. Crear asignaturas

```bash
curl -X POST "http://localhost:5000/api/asignaturas" \
  -H "Content-Type: application/json" \
  -d '{
    "codigo": "ISTG2001",
    "nombre": "Ingeniería de Software"
  }'
```

### 3. Crear propuesta

```bash
curl -X POST "http://localhost:5000/api/propuestas" \
  -H "Content-Type: application/json" \
  -d '{
    "nombreProyecto": "Mi Proyecto",
    "numeroParticipantes": 3,
    "profesorId": 1,
    "descripcion": "Descripción",
    "objetivo": "Objetivo",
    "alcance": "Alcance",
    "asignaturaIds": [1]
  }'
```

### 4. Cambiar estado

```bash
curl -X PATCH "http://localhost:5000/api/propuestas/1/estado" \
  -H "Content-Type: application/json" \
  -d '{
    "estado": "PENDIENTE"
  }'
```

---

## 🐛 TROUBLESHOOTING

**Error: Connection refused**

- Verifica que la API esté corriendo: `dotnet run`
- Verifica el puerto: debe estar en 5000

**Error: 404 Not Found**

- Verifica que el ID existe en la BD
- Verifica la ruta: `/api/propuestas` (no `/propuestas`)

**Error: 500 Internal Server Error**

- Revisa los logs de la consola
- Verifica que la cadena de conexión PostgreSQL sea correcta
