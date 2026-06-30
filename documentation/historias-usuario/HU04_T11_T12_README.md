# HU04 T11-T12: Observaciones CPGIC en el Frontend

## Descripción General

Implementación de la visualización de observaciones CPGIC en el formulario de propuesta cuando el estado es OBSERVADA, incluyendo:

- **T11**: Visualizar observaciones en diseño amarillo (prototipo HTML)
- **T12**: Botón para reenviar propuesta corregida

## Cambios Implementados

### 1. Component TypeScript (formulario-propuesta.component.ts)

#### Nuevas Propiedades

```typescript
// HU04 T11-T13: Observaciones CPGIC
observaciones: any[] = []; // Lista de observaciones
tieneObservaciones = false; // Indica si hay observaciones
reEnviando = false; // Estado de reenvío
mostrarModalReenvio = false; // Modal de confirmación para reenvío
```

#### Nuevos Métodos

**cargarObservaciones(propuestaId: number)**

- Carga observaciones al editar propuesta OBSERVADA
- Se llama automáticamente en `cargarPropuesta()` si estado === 'OBSERVADA'
- Campos mostrados: observacion, realizadoPor, fechaObservacion

**reenviarPropuesta()**

- Abre modal de confirmación antes de reenviar
- Requiere que propuestaId esté disponible

**confirmarReenvio()**

- Ejecuta POST /api/propuestas/{id}/reenviar-despues-observaciones
- Limpia observaciones localmente
- Desactiva formulario
- Redirige a /tablero después de 1.5s

**cancelarReenvio()**

- Cierra el modal de reenvío

#### Cambios en cargarPropuesta()

```typescript
// HU04 T11: Cargar observaciones si propuesta está OBSERVADA
if (propuesta.estado === "OBSERVADA") {
  this.cargarObservaciones(id);
}
```

### 2. Service (propuesta.service.ts)

#### Nuevos Métodos HTTP

**obtenerObservaciones(propuestaId: number): Observable<any[]>**

```typescript
GET / api / observaciones / propuesta / { propuestaId };
```

Retorna array de observaciones con campos:

- id
- propuestaId
- observacion (texto)
- realizadoPor (quién realizó la observación)
- fechaObservacion (timestamp)

**reenviarDespuesDeObservaciones(id: number): Observable<Propuesta>**

```typescript
POST / api / propuestas / { id } / reenviar - despues - observaciones;
```

Limpia observaciones y cambia estado OBSERVADA → PENDIENTE

### 3. Template HTML (formulario-propuesta.component.html)

#### Banner de Observaciones

```html
<!-- HU04 T11: Banner de observaciones CPGIC -->
<div *ngIf="tieneObservaciones" class="alerta alerta-observaciones">
  <span class="icono">⚠️</span>
  <span class="texto">
    Esta propuesta tiene <strong>{{ observaciones.length }}</strong>
    observación(es) de CPGIC que requieren corrección
  </span>
</div>
```

#### Sección de Observaciones

```html
<!-- HU04 T11: Tarjetas de observaciones en diseño amarillo -->
<div *ngIf="tieneObservaciones" class="seccion-observaciones">
  <h3 class="titulo-observaciones">📋 Observaciones CPGIC</h3>

  <div *ngFor="let obs of observaciones" class="tarjeta-observacion">
    <div class="cabecera-obs">
      <span class="realizador">{{ obs.realizadoPor || 'CPGIC' }}</span>
      <span class="fecha"
        >{{ obs.fechaObservacion | date: 'dd/MM/yyyy HH:mm' }}</span
      >
    </div>
    <div class="contenido-obs">
      <p>{{ obs.observacion }}</p>
    </div>
  </div>
</div>
```

#### Botón de Reenvío

```html
<!-- HU04 T12: Botón de Reenviar propuesta después de correcciones -->
<div
  *ngIf="estadoPropuesta === 'OBSERVADA' && tieneObservaciones"
  class="grupo-botones-reenvio"
>
  <button
    type="button"
    class="boton boton-reenvio"
    (click)="reenviarPropuesta()"
    [disabled]="reEnviando"
    [class.cargando]="reEnviando"
  >
    <span *ngIf="!reEnviando">📤 Reenviar Propuesta Corregida</span>
    <span *ngIf="reEnviando">Reenviando...</span>
  </button>
  <p class="ayuda-reenvio">
    Al reenviar se limpiarán las observaciones y la propuesta volverá a estar en
    revisión
  </p>
</div>
```

#### Modal de Confirmación Reenvío

```html
<!-- HU04 T12: Modal de confirmación para reenvío -->
<div *ngIf="mostrarModalReenvio" class="modal-overlay">
  <div class="modal-contenido">
    <!-- Header, Body con 2 observaciones a limpiar, Footer con botones -->
  </div>
</div>
```

### 4. Estilos SCSS (formulario-propuesta.component.scss)

#### Variables de Color Diseño Amarillo

```scss
// Background: #fef9ec (crema claro)
// Border: #ffc107 (amarillo advertencia)
// Badge/Accent: #F3BD46 (oro)
// Text: #856404 (marrón oscuro)
```

#### Clases Principales

**.alerta-observaciones**

- Background #fef9ec con border 2px #ffc107
- Muestra conteo de observaciones

**.seccion-observaciones**

- Container principal en diseño amarillo
- Titulo con icono 📋
- Lista de tarjetas de observaciones

**.tarjeta-observacion**

- Background white con border-left 4px #F3BD46
- Layout: cabecera (realizador + fecha) + contenido (observación)
- Sombra suave para separación visual

**.grupo-botones-reenvio**

- Botón centrado (max-width 400px)
- Texto de ayuda en cursiva bajo el botón

**.boton-reenvio**

- Color fondo: #F3BD46 (oro)
- Color texto: #0E2240 (navy oscuro)
- Hover: #e8ad3f (oro más oscuro)

## Flujo de Uso

### Visualización de Observaciones (T11)

1. Usuario accede a editar propuesta en estado OBSERVADA
2. Componente carga propuesta y detecta `estado === 'OBSERVADA'`
3. Llamada a `obtenerObservaciones(propuestaId)`
4. Se renderizan observaciones en tarjetas amarillas
5. Usuario ve:
   - Banner ⚠️ de alerta
   - Lista de observaciones con quién y cuándo
   - Botón para reenviar después de correcciones

### Reenvío Propuesta Corregida (T12)

1. Usuario realiza correcciones en la propuesta
2. Usuario hace clic en "📤 Reenviar Propuesta Corregida"
3. Se abre modal de confirmación
4. Al confirmar:
   - POST /api/propuestas/{id}/reenviar-despues-observaciones
   - Backend: limpia observaciones, cambia estado OBSERVADA → PENDIENTE
   - Frontend:
     - Limpia array observaciones[]
     - Desactiva formulario
     - Muestra mensaje "✓ Propuesta reenviada a revisión exitosamente"
     - Redirige a /tablero después de 1.5s

## Dependencias Backend (Ya Implementadas)

### Endpoints Utilizados

- **GET** `/api/observaciones/propuesta/{propuestaId}` → Lista observaciones
- **POST** `/api/propuestas/{id}/reenviar-despues-observaciones` → Reenviar y limpiar

### Servicios Utilizados

- `ObservacionesCpgicService` con `ObtenerObservacionesPorPropuestaAsync()`
- `PropuestaService` con `ReenviarDespuesDeObservacionesAsync()`

### Base de Datos

- Tabla `observaciones_cpgic` con campos: id, propuesta_id, observacion, realizadoPor, fechaObservacion

## Estados de Propuesta Soportados

| Estado    | T11 - Visualizar | T12 - Reenviar | T11/T12 Visible |
| --------- | ---------------- | -------------- | --------------- |
| BORRADOR  | ❌ No            | ❌ No          | ❌ No           |
| PENDIENTE | ❌ No            | ❌ No          | ❌ No           |
| OBSERVADA | ✅ Sí            | ✅ Sí          | ✅ Sí           |
| APROBADA  | ❌ No            | ❌ No          | ❌ No           |
| RECHAZADA | ❌ No            | ❌ No          | ❌ No           |

## Validaciones

### Frontend

- Botón de reenvío solo visible si `estado === 'OBSERVADA' && tieneObservaciones > 0`
- Modal requiere confirmación explícita
- Desactiva botón durante reenvío (`reEnviando` = true)

### Backend (Ya Validado)

- Solo propuestas en estado OBSERVADA pueden reenviarse
- Requiere que existan observaciones asociadas
- Transacción atómica: limpia observaciones y cambia estado
- Logging automático de operación

## Comprobación de Implementación

### Compilación

✅ Frontend (Angular) - Compilación exitosa
✅ Backend (.NET 8) - Compilación exitosa

### Conexión API

- Endpoint observaciones: GET /api/observaciones/propuesta/{id}
- Endpoint reenvío: POST /api/propuestas/{id}/reenviar-despues-observaciones

### Interfaz Visual

- 📋 Observaciones en tarjetas amarillas (#fef9ec, #ffc107)
- ⚠️ Banner de alerta
- 📤 Botón de reenvío (color oro #F3BD46)
- Modal de confirmación con detalles de reenvío

## Next Steps - HU04 T13 (Integración en Formulario)

Próxima tarea: Mostrar observaciones inline en los campos afectados

- Detectar qué campos tienen observaciones
- Marcar visualmente campos con observaciones
- Mostrar tooltip o nota en cada campo afectado

## Notas Técnicas

- **Responsivo**: Soporta dispositivos móviles (<600px width)
- **Animaciones**: fadeIn (modal), slideUp (contenido)
- **Accesibilidad**: Uso de aria labels y contraste de colores
- **Performance**: Carga observaciones solo si estado === OBSERVADA
- **Manejo de errores**: Silencioso si no hay observaciones (normal en BORRADOR/PENDIENTE)

## Referencias

- Prototipo HTML: Diseño amarillo con cards #fef9ec, border #ffc107
- EPN Colors: Navy #0E2240, Gold #F3BD46, Blue #77A4DC
- Badge OBSERVADA: #F3BD46
- Fecha formato: dd/MM/yyyy HH:mm (Angular date pipe)
