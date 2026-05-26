# Flujos del Sistema

## Flujo General de Estados de Propuesta

```
                    ┌─────────────┐
                    │  BORRADOR   │  Editable libremente
                    └──────┬──────┘
                           │ [Enviar a Revisión]
                           │ (HU03 T10)
                           ▼
                    ┌─────────────┐
                    │  PENDIENTE  │  Esperando revisión CPGIC
                    └──────┬──────┘
                           │
                    ┌──────┴──────────┐
                    │                 │
         [HU04 T12-T13]    [HU04 T11 - Sin Problemas]
          Crear Obs.              Aprobar
                    │                 │
                    ▼                 ▼
            ┌──────────────┐    ┌──────────────┐
            │  OBSERVADA   │    │  APROBADA    │  Listo para estudiantes
            └──────┬───────┘    └──────┬───────┘
                   │                   │
      [HU04 T15]   │            [HU07 T20-T22]
   Reenviar después│            Asignar estudiantes
   de correcciones │                   │
                   │            ┌──────┴──────┐
                   │            │             │
                   │     [Cambios]   [Sin cambios]
                   │            │             │
                   │            ▼             │
                   │      ┌──────────┐        │
                   │      │ PENDIENTE│◄───────┘
                   │      └──────────┘
                   │            ▲
                   └────────────┘

Estados Finales:
├─ RECHAZADA (si CPGIC rechaza)
├─ APROBADA (estable, con estudiantes asignados)
└─ ELIMINADA [HU08] (solo desde BORRADOR)
```

---

## Flujo Detallado HU01-HU03 (Registro y Envío)

```
PROFESOR PROPONE PROYECTO
│
├─ 1. Entra a sistema
│    └─ Ve formulario vacío
│
├─ 2. Completa formulario
│    ├─ Título: "App Gestión Tareas"
│    ├─ Descripción, Objetivo, Alcance
│    ├─ Selecciona Asignaturas (1-N)
│    ├─ Selecciona Docente Proponente (1:1)
│    └─ Número de participantes
│
├─ 3. Click "Guardar Borrador"
│    └─ [HU01 T01-T05]
│       POST /api/propuestas
│       ↓
│       Estado: BORRADOR (¡Editable!)
│
├─ 4. (Opcional) Edita propuesta
│    └─ [HU05 T16-T17]
│       PUT /api/propuestas/{id}
│       ↓
│       Sigue en BORRADOR
│
└─ 5. Envía a Revisión
     └─ [HU03 T10]
        POST /api/propuestas/{id}/enviar-revision
        ↓
        Estado: PENDIENTE (¡No editable!)
        FechaEnvioRevision: timestamp
```

**Transiciones Posibles desde BORRADOR**:
- ✅ → PENDIENTE (enviar-revision)
- ✅ → BORRADOR (editar)
- ✅ → ❌ ELIMINADA [HU08] (delete)

---

## Flujo Detallado HU04 (Observaciones)

```
CPGIC REVISA PROPUESTA EN PENDIENTE
│
├─ 1. Ve propuesta en PENDIENTE
│    └─ [HU02 Tablero]
│       Accesos: Ver Detalle, Agregar Observación
│
├─ 2A. Si no hay problemas
│      └─ [HU04 T11 - Aprobar]
│         PUT /api/propuestas/{id}/cambiar-estado
│         { "nuevoEstado": "APROBADA" }
│         ↓
│         Estado: APROBADA
│         → Siguiente: HU07 (Asignar estudiantes)
│
└─ 2B. Si hay problemas
       └─ [HU04 T12-T13]
          POST /api/observaciones
          {
            "propuestaId": id,
            "descripcion": "Aclarar arquitectura...",
            "tipoObservacion": "Técnica"
          }
          ↓
          Estado automático: PENDIENTE → OBSERVADA
          Observación registrada
          │
          ├─ 3. Profesor ve observaciones
          │     └─ [HU06 T18-T19]
          │        GET /api/propuestas/{id}/detalle
          │        ↓
          │        Incluye "observaciones" array
          │
          ├─ 4. Profesor edita (Correcciones)
          │     └─ [HU05 T16-T17]
          │        Solo editable en OBSERVADA
          │        PUT /api/propuestas/{id}
          │
          └─ 5. Reenvia
                └─ [HU04 T15]
                   POST /api/propuestas/{id}/reenviar-despues-observaciones
                   ↓
                   Estado: OBSERVADA → PENDIENTE
                   Observaciones se mantienen (en histórico)
                   ↓
                   Vuelve a paso 1 (revisa de nuevo CPGIC)

NOTA: Puede haber ciclo OBSERVADA ↔ PENDIENTE múltiples veces
```

**Transiciones desde OBSERVADA**:
- ✅ → PENDIENTE (reenviar-despues-observaciones)
- ✅ → BORRADOR (editar)
- ✅ → ❌ ELIMINADA [HU08 NO PERMITE - solo BORRADOR]

---

## Flujo Detallado HU06 (Detalle Completo)

```
USUARIO REQUIERE VER TODO DE UNA PROPUESTA
│
└─ GET /api/propuestas/{id}/detalle
   │
   ├─ 1. Datos Básicos
   │    ├─ id, titulo, descripcion, objetivo, alcance
   │    └─ estado, fechas (creacion, envio, actualizacion)
   │
   ├─ 2. Profesor Proponente
   │    ├─ nombre, correo, titulo_academico
   │    └─ departamento
   │
   ├─ 3. Asignaturas Asociadas
   │    ├─ codigo, nombre, creditos, semestre
   │    └─ horas_dedicacion (si aplica)
   │
   ├─ 4. Observaciones CPGIC (si las hay)
   │    ├─ descripcion, tipo, fecha_creacion
   │    └─ usuario_creador
   │
   ├─ 5. Estudiantes Asignados (si los hay)
   │    ├─ nombre, apellido, correo, carrera
   │    └─ fecha_asignacion, asignado_por
   │
   └─ 6. Histórico de Estados
       ├─ BORRADOR → PENDIENTE (fecha)
       ├─ PENDIENTE → OBSERVADA (fecha)
       └─ OBSERVADA → PENDIENTE (fecha)

NOTA: Esta es la "vista de verdad" - incluye TODO
```

---

## Flujo Detallado HU07 (Asignación de Estudiantes)

```
PROPUESTA APROBADA - ASIGNAR ESTUDIANTES
│
├─ Pre-requisito: Estado APROBADA o PENDIENTE
│
├─ 1. Profesor abre modal "Asignar Estudiantes"
│    └─ [HU07 T21]
│       Modal muestra:
│       - Campo de búsqueda
│       - Lista de estudiantes disponibles
│       - Chips de seleccionados
│       - Campo "motivo" (requerido si cambios)
│
├─ 2. Busca estudiantes
│    └─ [HU07 T22]
│       GET /api/estudiantes/buscar?searchTerm=Juan
│       ↓
│       Retorna coincidencias en tiempo real
│       (Debounce: 300ms)
│
├─ 3. Selecciona máximo 5
│    └─ Validación frontend: error si > 5
│
├─ 4. Click "Asignar"
│    └─ [HU07 T20]
│       POST /api/estudiantes/{propuestaId}/asignar
│       {
│         "estudianteIds": [1, 2, 3],
│         "motivo": "Seleccionados por desempeño",
│         "realizadoPor": "profesor@example.com"
│       }
│       ↓
│       Validaciones:
│       - Max 5 estudiantes ✓
│       - Propuesta APROBADA o PENDIENTE ✓
│       - Motivo requerido ✓
│       ↓
│       SI propuesta estaba APROBADA:
│         Estado: APROBADA → PENDIENTE (automático)
│         Motivo: "Se asignaron estudiantes"
│       SI estaba PENDIENTE:
│         Se mantiene PENDIENTE
│
├─ 5. (Opcional) Si APROBADA → PENDIENTE
│    └─ [HU07 T25]
│       Profesor ve botón: "Solicitar Nueva Aprobación"
│       POST /api/propuestas/{id}/solicitar-nueva-aprobacion
│       {
│         "motivo": "Se asignaron 3 estudiantes..."
│       }
│       ↓
│       Vuelve a PENDIENTE (de nuevo)
│       Para que CPGIC apruebe con estudiantes asignados

NOTA: Ciclo puede ser:
  APROBADA
    ↓ [asignar estudiantes]
  PENDIENTE
    ↓ [solicitar nueva aprobacion]
  PENDIENTE (espera CPGIC)
    ↓ [CPGIC aprueba]
  APROBADA (con estudiantes)
```

**Transiciones desde APROBADA**:
- ✅ → PENDIENTE (asignar estudiantes → automático)
- ✅ → PENDIENTE (solicitar-nueva-aprobacion)
- ✅ → RECHAZADA (si CPGIC rechaza)

---

## Flujo Detallado HU08 (Eliminación)

```
PROFESOR QUIERE ELIMINAR PROPUESTA
│
├─ Condición: SOLO BORRADOR
│  (PENDIENTE, OBSERVADA, APROBADA, RECHAZADA → NO PERMITIDO)
│
├─ 1. Ve propuesta en BORRADOR
│    └─ Botón "Eliminar" visible
│
├─ 2. Click "Eliminar"
│    └─ [HU08 T27-T28]
│       Modal de confirmación:
│       ├─ Nombre propuesta
│       ├─ Estado: BORRADOR
│       ├─ Advertencia: "NO se puede deshacer"
│       └─ 2 botones: Cancelar, Confirmar
│
├─ 3. Click "Confirmar"
│    └─ DELETE /api/propuestas/{id}
│       ↓
│       Validación: estado == BORRADOR
│       ↓
│       Cascada elimina:
│       ├─ propuesta_asignaturas
│       ├─ propuesta_estudiantes
│       ├─ observaciones_cpgic
│       └─ historial_estados
│       ↓
│       Status 200 OK
│       {
│         "message": "Propuesta eliminada exitosamente",
│         "propuestaId": 42
│       }
│
└─ 4. Redirige a tablero
     └─ Propuesta ya no aparece en listados

FLUJO DE ERROR:

Intenta eliminar PENDIENTE
├─ DELETE /api/propuestas/{id}
├─ Validación: estado == BORRADOR → FAIL
├─ Status 403 Forbidden
└─ Mensaje: "Solo se pueden eliminar propuestas en BORRADOR"

Intenta eliminar con ID inválido
├─ DELETE /api/propuestas/9999
├─ Validación: propuesta existe → FAIL
├─ Status 404 Not Found
└─ Mensaje: "Propuesta no encontrada"
```

**Estados que PERMITEN eliminación**:
- ✅ BORRADOR

**Estados que NO PERMITEN eliminación**:
- ❌ PENDIENTE (403 Forbidden)
- ❌ OBSERVADA (403 Forbidden)
- ❌ APROBADA (403 Forbidden)
- ❌ RECHAZADA (403 Forbidden)

---

## Matriz de Transiciones de Estados

```
              ┌──────────────────────────────────────────────────────┐
              │           ESTADO DESTINO                             │
┌─────────────┼──────────────────────────────────────────────────────┤
│ ESTADO      │ BORRADOR │ PENDIENTE │ OBSERVADA │ APROBADA │RECHAZA │
│ ORIGEN      │          │           │           │          │DA      │
├─────────────┼──────────┼───────────┼───────────┼──────────┼────────┤
│ BORRADOR    │ ✓Edit    │ ✓Enviar   │ ✗         │ ✗        │ ✗      │
│             │ ✓Delete  │ (HU03)    │           │          │        │
├─────────────┼──────────┼───────────┼───────────┼──────────┼────────┤
│ PENDIENTE   │ ✗        │ ✓Edit     │ ✓Observar │ ✓Aprobar │ ✓Rech. │
│             │          │ (HU05)    │ (HU04)    │ (CPGIC)  │(CPGIC) │
├─────────────┼──────────┼───────────┼───────────┼──────────┼────────┤
│ OBSERVADA   │ ✓Edit    │ ✓Reenviar │ ✓Edit     │ ✗        │ ✗      │
│             │ (HU05)   │ (HU04)    │ (HU05)    │          │        │
├─────────────┼──────────┼───────────┼───────────┼──────────┼────────┤
│ APROBADA    │ ✗        │ ✓Asignar  │ ✗         │ ✓Mantener│ ✗      │
│             │          │ (HU07)    │           │ (HU07)   │        │
├─────────────┼──────────┼───────────┼───────────┼──────────┼────────┤
│ RECHAZADA   │ ✗        │ ✗         │ ✗         │ ✗        │ -      │
│ (Terminal)  │          │           │           │          │        │
└─────────────┴──────────┴───────────┴───────────┴──────────┴────────┘
```

---

## Secuencia Temporal Típica

```
Día 1 (Lunes)
┌─────────────────────────────────────────────────────────┐
│ 09:00 Profesor crea propuesta en BORRADOR              │
│ 09:15 Profesor edita y completa información            │
│ 09:30 Profesor envía a revisión → PENDIENTE            │
└─────────────────────────────────────────────────────────┘
       ↓
Día 2-3 (Martes-Miércoles)
┌─────────────────────────────────────────────────────────┐
│ 10:00 CPGIC revisa propuesta                           │
│ 11:00 CPGIC agrega observaciones → OBSERVADA           │
│ 11:15 Email notifica al profesor                       │
└─────────────────────────────────────────────────────────┘
       ↓
Día 4 (Jueves)
┌─────────────────────────────────────────────────────────┐
│ 14:00 Profesor edita respuesta a observaciones        │
│ 14:30 Profesor reenvia → PENDIENTE                     │
└─────────────────────────────────────────────────────────┘
       ↓
Día 5 (Viernes)
┌─────────────────────────────────────────────────────────┐
│ 09:00 CPGIC revisa correcciones                        │
│ 09:45 CPGIC aprueba → APROBADA                         │
│ 10:00 Email notifica al profesor                       │
└─────────────────────────────────────────────────────────┘
       ↓
Día 6-7 (Siguiente semana)
┌─────────────────────────────────────────────────────────┐
│ 10:00 Profesor asigna estudiantes (max 5)             │
│ 10:15 Propuesta → PENDIENTE (solicita revisión)        │
│ 10:30 Solicita nueva aprobación (CPGIC rápida)        │
│ 11:00 CPGIC aprueba → APROBADA (con estudiantes)      │
└─────────────────────────────────────────────────────────┘

Total: ~1-2 semanas de ciclo completo
```

---

## Consideraciones de Diseño

### ¿Por qué APROBADA → PENDIENTE al asignar estudiantes?

**Regla**: Si hay cambios en propuesta APROBADA, vuelve a revisión.

**Justificación**:
- Asignar estudiantes es un cambio significativo
- CPGIC necesita validar que los estudiantes son apropiados
- Evita aprobaciones "endurecidas" en APROBADA

### ¿Por qué no permite editar PENDIENTE?

**Regla**: Solo BORRADOR y OBSERVADA son editables.

**Justificación**:
- PENDIENTE = "bajo revisión de CPGIC"
- Cambios durante revisión crean confusión
- Si hay cambios necesarios → agregar observaciones
- Profesor corrige en OBSERVADA

### ¿Por qué solo eliminar BORRADOR?

**Regla**: BORRADOR = "borrador privado, no revisado"

**Justificación**:
- PENDIENTE+ = "en proceso/aprobadas", no se deben eliminar
- Integridad de histórico CPGIC
- Auditoría: todo debe quedar registrado
- Si propuesta no sirve → rechazar, no eliminar

---
