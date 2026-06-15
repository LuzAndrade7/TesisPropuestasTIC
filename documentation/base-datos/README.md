# Base de Datos - Documentación Técnica

## Resumen

PostgreSQL 16 en Neon Cloud con 8 tablas principales conectadas por relaciones N:1 y N:N con cascadas automáticas.

---

## Diagrama ER (Textual)

```
┌─────────────────────────────┐
│         DOCENTES            │
│ (Profesores proponentes)    │
├─────────────────────────────┤
│ id (PK)                     │
│ nombre                      │
│ correo (UNIQUE)             │
│ titulo_academico            │
│ departamento                │
└────────┬────────────────────┘
         │
         │ 1:N
         │
┌────────▼────────────────────┐          ┌──────────────────┐
│      PROPUESTAS             │          │   ASIGNATURAS    │
│ (Núcleo del sistema)        │          │                  │
├─────────────────────────────┤          ├──────────────────┤
│ id (PK)                     │          │ id (PK)          │
│ titulo                      │          │ codigo (UNIQUE)  │
│ descripcion                 │          │ nombre           │
│ objetivo                    │          │ creditos         │
│ alcance                     │          │ semestre         │
│ numero_participantes        │          └──────────────────┘
│ estado (BORRADOR/...)       │                  △
│ fecha_creacion              │                  │
│ fecha_envio_revision        │                  │ N:N
│ fecha_actualizacion         │                  │
│ docente_id (FK)             │          ┌───────┴──────────┐
└─────┬───────────────────────┘          │                  │
      │                          ┌───────▼──────────────┐
      │                          │PROPUESTA_ASIGNATURAS│
      │                          │ (Junction Table)     │
      │                          ├─────────────────────┤
      │ 1:N                      │ propuesta_id (FK)   │
      │                          │ asignatura_id (FK)  │
      │                          │ horas_dedicacion    │
      │                          └─────────────────────┘
      │
      ├─→ 1:N ─────────────────────────────────────────────┐
      │                                                     │
      │                    ┌────────────────────────────┐   │
      │                    │ PROPUESTA_ESTUDIANTES      │   │
      │ (HU07 T20)         │ (Students Assignment)      │   │
      │                    ├────────────────────────────┤   │
      │                    │ id (PK)                    │   │
      │                    │ propuesta_id (FK)   ◄──────┤   │
      │                    │ estudiante_id (FK) ─┐     │   │
      │                    │ fecha_asignacion   │     │   │
      │                    │ asignado_por       │     │   │
      │                    │ estado (ACTIVO)    │     │   │
      │                    │ UNIQUE(prop,est)   │     │   │
      │                    └────────────────────┘     │   │
      │                                               │   │
      │    ┌──────────────────────────────────┐      │   │
      │    │       ESTUDIANTES                │      │   │
      │    │                                  │      │   │
      │    ├──────────────────────────────────┤      │   │
      │    │ id (PK)                          │      │   │
      │    │ nombre                           │      │   │
      │    │ apellido                         │      │   │
      │    │ correo (UNIQUE)                  │      │   │
      │    │ carrera                          │      │   │
      │    │ semestre                         │      │   │
      │    │ PropuestaEstudiantes ◄───────────┤──────┘   │
      │    └──────────────────────────────────┘          │
      │                                                   │
      └─→ 1:N ────────────────────────────────────────────┤
      │                                                   │
      │    ┌────────────────────────────────────┐         │
      │    │ OBSERVACIONES_CPGIC                │         │
      │    │ (HU04 Feedback)                    │         │
      │    ├────────────────────────────────────┤         │
      │    │ id (PK)                            │         │
      │    │ propuesta_id (FK)          ◄───────┤─────────┘
      │    │ descripcion                        │
      │    │ tipo_observacion                   │
      │    │ fecha_creacion                     │
      │    │ usuario_creador                    │
      │    └────────────────────────────────────┘
      │
      └─→ 1:N ──────────────────────────────┐
             (Estado Change History)        │
             ┌─────────────────────┐        │
             │ HISTORIAL_ESTADOS   │        │
             ├─────────────────────┤        │
             │ id (PK)             │        │
             │ propuesta_id (FK)   │◄───────┘
             │ estado_anterior     │
             │ estado_nuevo        │
             │ fecha_cambio        │
             │ motivo              │
             │ usuario             │
             └─────────────────────┘
```

---

## Tablas Detalladas

### 1. **PROPUESTAS** (Tabla Principal)

```sql
CREATE TABLE propuestas (
  id SERIAL PRIMARY KEY,
  titulo VARCHAR(255) NOT NULL,
  descripcion TEXT NOT NULL,
  objetivo TEXT NOT NULL,
  alcance TEXT NOT NULL,
  numero_participantes INT NOT NULL CHECK (numero_participantes > 0),

  -- Estados: BORRADOR, PENDIENTE, OBSERVADA, APROBADA, RECHAZADA
  estado VARCHAR(50) NOT NULL DEFAULT 'BORRADOR',

  -- FK
  docente_id INT NOT NULL REFERENCES docentes(id) ON DELETE CASCADE,

  -- Fechas
  fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  fecha_envio_revision TIMESTAMP,
  fecha_actualizacion TIMESTAMP,

  -- Índices
  UNIQUE(titulo),
  INDEX idx_propuestas_estado (estado),
  INDEX idx_propuestas_docente_id (docente_id)
);
```

**Estados Posibles**:

- `BORRADOR` - Editable, no visible en tablero de revisión
- `PENDIENTE` - Esperando revisión CPGIC
- `OBSERVADA` - Requiere correcciones
- `APROBADA` - Listo para asignar estudiantes
- `RECHAZADA` - Rechazada definitivamente

**Ciclo de Vida**:

```
BORRADOR  →[enviar]→  PENDIENTE
   ↑                      ↓
   └─[correcciones]← OBSERVADA
                          ↓
                     [aprobada]
                          ↓
                      APROBADA
                          ↓
                   [asignar estudiantes]
                   [si cambios → PENDIENTE]
```

---

### 2. **DOCENTES**

```sql
CREATE TABLE docentes (
  id SERIAL PRIMARY KEY,
  nombre VARCHAR(200) NOT NULL,
  correo VARCHAR(255) NOT NULL UNIQUE,
  titulo_academico VARCHAR(100),
  departamento VARCHAR(100),

  INDEX idx_docentes_correo (correo)
);
```

**Relaciones**:

- 1:N → PROPUESTAS (un docente puede tener múltiples propuestas)

---

### 3. **ASIGNATURAS**

```sql
CREATE TABLE asignaturas (
  id SERIAL PRIMARY KEY,
  codigo VARCHAR(20) NOT NULL UNIQUE,
  nombre VARCHAR(200) NOT NULL,
  descripcion TEXT,
  creditos INT,
  semestre INT,

  INDEX idx_asignaturas_codigo (codigo)
);
```

**Relaciones**:

- N:N → PROPUESTAS (a través de PROPUESTA_ASIGNATURAS)

---

### 4. **PROPUESTA_ASIGNATURAS** (Junction Table)

```sql
CREATE TABLE propuesta_asignaturas (
  propuesta_id INT NOT NULL,
  asignatura_id INT NOT NULL,
  horas_dedicacion INT,

  PRIMARY KEY (propuesta_id, asignatura_id),
  FOREIGN KEY (propuesta_id) REFERENCES propuestas(id) ON DELETE CASCADE,
  FOREIGN KEY (asignatura_id) REFERENCES asignaturas(id) ON DELETE CASCADE,

  INDEX idx_pa_propuesta (propuesta_id),
  INDEX idx_pa_asignatura (asignatura_id)
);
```

**Validaciones**:

- UNIQUE(propuesta_id, asignatura_id) - No duplicadas
- CASCADE elimina automáticamente cuando propuesta se elimina

---

### 5. **ESTUDIANTES** (HU07)

```sql
CREATE TABLE estudiantes (
  id SERIAL PRIMARY KEY,
  nombre VARCHAR(200) NOT NULL,
  apellido VARCHAR(200) NOT NULL,
  correo VARCHAR(255) NOT NULL UNIQUE,
  carrera VARCHAR(200),
  semestre INT,

  INDEX idx_estudiantes_correo (correo),
  INDEX idx_estudiantes_nombre (nombre),
  INDEX idx_estudiantes_apellido (apellido)
);
```

**Relaciones**:

- N:N → PROPUESTAS (a través de PROPUESTA_ESTUDIANTES)

---

### 6. **PROPUESTA_ESTUDIANTES** (HU07 T20)

```sql
CREATE TABLE propuesta_estudiantes (
  id SERIAL PRIMARY KEY,
  propuesta_id INT NOT NULL,
  estudiante_id INT NOT NULL,

  -- Metadatos
  fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  asignado_por VARCHAR(255),
  estado VARCHAR(50) DEFAULT 'ACTIVO',  -- ACTIVO, INACTIVO, COMPLETADO

  -- Constraints
  UNIQUE(propuesta_id, estudiante_id),  -- Max 1 asignación por estudiante
  FOREIGN KEY (propuesta_id) REFERENCES propuestas(id) ON DELETE CASCADE,
  FOREIGN KEY (estudiante_id) REFERENCES estudiantes(id) ON DELETE CASCADE,

  -- Índices
  INDEX idx_pe_propuesta (propuesta_id),
  INDEX idx_pe_estudiante (estudiante_id),
  INDEX idx_pe_composite (propuesta_id, estado)
);
```

**Limitaciones**:

- Máximo 5 estudiantes por propuesta (validado en servicio)
- UNIQUE garantiza no duplicar estudiante en misma propuesta

---

### 7. **OBSERVACIONES_CPGIC** (HU04)

```sql
CREATE TABLE observaciones_cpgic (
  id SERIAL PRIMARY KEY,
  propuesta_id INT NOT NULL,
  descripcion TEXT NOT NULL,
  tipo_observacion VARCHAR(100),  -- Técnica, Formato, Alcance, etc

  -- Auditoría
  fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  usuario_creador VARCHAR(255),

  FOREIGN KEY (propuesta_id) REFERENCES propuestas(id) ON DELETE CASCADE,
  INDEX idx_oc_propuesta (propuesta_id)
);
```

**Efecto**: Cuando se crea observación:

1. Propuesta cambia: PENDIENTE → OBSERVADA
2. Observación se registra en histórico

---

### 8. **HISTORIAL_ESTADOS** (Auditoría)

```sql
CREATE TABLE historial_estados (
  id SERIAL PRIMARY KEY,
  propuesta_id INT NOT NULL,
  estado_anterior VARCHAR(50),
  estado_nuevo VARCHAR(50) NOT NULL,

  -- Metadatos
  fecha_cambio TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  motivo VARCHAR(500),
  usuario VARCHAR(255),

  FOREIGN KEY (propuesta_id) REFERENCES propuestas(id) ON DELETE CASCADE,
  INDEX idx_he_propuesta (propuesta_id),
  INDEX idx_he_fecha (fecha_cambio)
);
```

**Propósito**: Auditoría completa de cambios de estado
**Beneficio**: Trazabilidad 100% de transiciones

---

## Cascadas ON DELETE

| Tabla Padre | Tabla Hija            | Acción  |
| ----------- | --------------------- | ------- |
| PROPUESTAS  | PROPUESTA_ASIGNATURAS | CASCADE |
| PROPUESTAS  | PROPUESTA_ESTUDIANTES | CASCADE |
| PROPUESTAS  | OBSERVACIONES_CPGIC   | CASCADE |
| PROPUESTAS  | HISTORIAL_ESTADOS     | CASCADE |
| DOCENTES    | PROPUESTAS            | CASCADE |
| ASIGNATURAS | PROPUESTA_ASIGNATURAS | CASCADE |
| ESTUDIANTES | PROPUESTA_ESTUDIANTES | CASCADE |

**Implicación**: Cuando se elimina propuesta:

```
DELETE propuestas WHERE id = 42
  → Automático: DELETE propuesta_asignaturas WHERE propuesta_id = 42
  → Automático: DELETE propuesta_estudiantes WHERE propuesta_id = 42
  → Automático: DELETE observaciones_cpgic WHERE propuesta_id = 42
  → Automático: DELETE historial_estados WHERE propuesta_id = 42
```

---

## Índices para Optimización

```sql
-- Búsquedas por estado (filtros en tablero)
CREATE INDEX idx_propuestas_estado ON propuestas(estado);

-- Búsquedas por docente
CREATE INDEX idx_propuestas_docente_id ON propuestas(docente_id);

-- Búsquedas por asignatura
CREATE INDEX idx_pa_propuesta ON propuesta_asignaturas(propuesta_id);
CREATE INDEX idx_pa_asignatura ON propuesta_asignaturas(asignatura_id);

-- Búsquedas de estudiantes en propuesta
CREATE INDEX idx_pe_propuesta ON propuesta_estudiantes(propuesta_id);
CREATE INDEX idx_pe_estudiante ON propuesta_estudiantes(estudiante_id);
CREATE INDEX idx_pe_composite ON propuesta_estudiantes(propuesta_id, estado);

-- Búsquedas de observaciones
CREATE INDEX idx_oc_propuesta ON observaciones_cpgic(propuesta_id);

-- Búsquedas de histórico
CREATE INDEX idx_he_propuesta ON historial_estados(propuesta_id);
CREATE INDEX idx_he_fecha ON historial_estados(fecha_cambio);

-- Búsquedas por correo (usuario único)
CREATE INDEX idx_docentes_correo ON docentes(correo);
CREATE INDEX idx_estudiantes_correo ON estudiantes(correo);
CREATE INDEX idx_asignaturas_codigo ON asignaturas(codigo);
```

---

## Queries Frecuentes

### 1. Listar propuestas por estado

```sql
-- Frontend Tablero (HU02)
SELECT * FROM propuestas
WHERE estado = 'PENDIENTE'
ORDER BY fecha_creacion DESC;
```

### 2. Detalle completo de propuesta (HU06)

```sql
-- Con EAGER LOADING de todas las relaciones
SELECT p.*, d.nombre as docente_nombre, d.correo as docente_correo
FROM propuestas p
LEFT JOIN docentes d ON p.docente_id = d.id
WHERE p.id = 42;

SELECT a.* FROM asignaturas a
INNER JOIN propuesta_asignaturas pa ON a.id = pa.asignatura_id
WHERE pa.propuesta_id = 42;

SELECT e.*, pe.fecha_asignacion, pe.asignado_por
FROM estudiantes e
INNER JOIN propuesta_estudiantes pe ON e.id = pe.estudiante_id
WHERE pe.propuesta_id = 42 AND pe.estado = 'ACTIVO';

SELECT * FROM observaciones_cpgic
WHERE propuesta_id = 42
ORDER BY fecha_creacion DESC;
```

### 3. Buscar estudiantes disponibles (HU07 T22)

```sql
-- Estudiantes NO asignados a propuesta 42
SELECT e.* FROM estudiantes e
WHERE e.id NOT IN (
  SELECT DISTINCT estudiante_id
  FROM propuesta_estudiantes
  WHERE propuesta_id = 42 AND estado = 'ACTIVO'
)
ORDER BY e.apellido, e.nombre;
```

### 4. Contar estudiantes asignados

```sql
-- Para validar máximo 5
SELECT COUNT(*) as total_asignados
FROM propuesta_estudiantes
WHERE propuesta_id = 42 AND estado = 'ACTIVO';
```

### 5. Histórico de cambios

```sql
-- Ver todos los cambios de estado de una propuesta
SELECT * FROM historial_estados
WHERE propuesta_id = 42
ORDER BY fecha_cambio DESC;
```

### 6. Búsqueda por nombre

```sql
-- Buscar estudiantes (case-insensitive)
SELECT * FROM estudiantes
WHERE LOWER(nombre) LIKE LOWER('%juan%')
   OR LOWER(apellido) LIKE LOWER('%juan%')
   OR LOWER(correo) LIKE LOWER('%juan%')
ORDER BY apellido, nombre;
```

---

## Integridad Referencial

### Validaciones a Nivel BD

```sql
-- Check: número participantes positivo
CHECK (numero_participantes > 0)

-- Check: estado válido
CHECK (estado IN ('BORRADOR', 'PENDIENTE', 'OBSERVADA', 'APROBADA', 'RECHAZADA'))

-- Unique: No propuestas duplicadas
UNIQUE(titulo)

-- Unique: No estudiantes duplicados por asignación
UNIQUE(propuesta_id, estudiante_id)

-- Unique: Correos únicos
UNIQUE(correo) [en docentes, estudiantes, asignaturas]

-- Unique: Códigos únicos
UNIQUE(codigo) [en asignaturas]
```

### Validaciones a Nivel Aplicación (Backend)

- Estado válido antes de guardar
- Máximo 5 estudiantes (no en BD, en lógica)
- Campos requeridos no nulos
- Docente/Asignatura/Estudiante existe

---

## Migraciones y Scripts

Ubicación: `/database/`

```
sprint1_init.sql              (Creación tablas, data inicial)
sprint2_add_estudiantes.sql   (Agregó tabla estudiantes)
sprint3_add_historia.sql      (Agregó historial_estados)
sprint4_propuesta_estudiantes.sql  (HU07 - Junction table)
```

---

## Información de Conexión

**Host**: ep-lingering-mouse-aqf2l271-pooler.c-8.us-east-1.aws.neon.tech  
**Database**: testistic  
**Port**: 5432 (PostgreSQL estándar)  
**User**: [Configurado en environment variables]  
**Version**: PostgreSQL 16

---

## Backup y Recuperación

```bash
# Backup
pg_dump -h HOST -U USER -d DATABASE > backup.sql

# Restore
psql -h HOST -U USER -d DATABASE < backup.sql
```

---

## Conclusión

La base de datos implementa un modelo relacional completo con:

- ✅ Integridad referencial (FK + CASCADE)
- ✅ Validaciones a nivel BD (CHECK, UNIQUE)
- ✅ Índices para performance
- ✅ Auditoría completa (historial_estados)
- ✅ Soporte N:N (junction tables)
- ✅ Escalabilidad futura

---
