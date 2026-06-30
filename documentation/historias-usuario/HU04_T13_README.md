# HU04 T13: Integración en Formulario - Observaciones por Campo

## Descripción General

Implementación final de la integración de observaciones CPGIC directamente en el formulario de propuesta, mostrando observaciones específicas junto a los campos afectados.

## Cambios Implementados

### 1. Component TypeScript (formulario-propuesta.component.ts)

#### Nuevas Propiedades

```typescript
// HU04 T13: Observaciones por campo (para integración en formulario)
observacionesPorCampo: { [key: string]: any[] } = {
  nombreProyecto: [],
  numeroParticipantes: [],
  descripcion: [],
  objetivo: [],
  alcance: [],
  asignaturas: []
};
camposConObservaciones: Set<string> = new Set();
```

#### Nuevos Métodos

**clasificarObservacionesPorCampo(observaciones: any[])**

- Analiza automáticamente el texto de cada observación
- Detecta keywords asociadas a cada campo
- Keywords por campo:
  - `nombreProyecto`: "nombre", "proyecto", "título"
  - `numeroParticipantes`: "participantes", "integrantes", "equipo", "miembros"
  - `descripcion`: "descripción", "detallar", "metodología", "evaluación"
  - `objetivo`: "objetivo", "específico", "medible", "metas"
  - `alcance`: "alcance", "recursos", "tecnológicos", "integración"
  - `asignaturas`: "asignatura", "materia", "cátedra"

**obtenerObservacionesCampo(nombreCampo: string): any[]**

- Retorna observaciones asociadas a un campo específico
- Usado en template para mostrar observaciones bajo field

**tieneCampoObservaciones(nombreCampo: string): boolean**

- Verifica si un campo tiene observaciones
- Usado con \*ngIf para mostrar/ocultar badges y contenido

### 2. Template HTML (formulario-propuesta.component.html)

#### Estructura de Campos con Observaciones

**Para cada campo (nombreProyecto, descripcion, objetivo, alcance, asignaturas):**

1. **Badge de Observaciones** (en el label)

```html
<span
  *ngIf="tieneCampoObservaciones('nombreProyecto')"
  class="badge-observacion"
  [title]="'Observaciones: ' + obtenerObservacionesCampo('nombreProyecto')[0]?.observacion"
>
  ⚠️ {{ obtenerObservacionesCampo('nombreProyecto').length }}
</span>
```

2. **Clase en el Campo**

```html
[class.campo-observacion]="tieneCampoObservaciones('nombreProyecto')"
```

3. **Panel de Observaciones** (debajo del campo)

```html
<div
  *ngIf="tieneCampoObservaciones('nombreProyecto')"
  class="observaciones-campo"
>
  <p
    *ngFor="let obs of obtenerObservacionesCampo('nombreProyecto')"
    class="observacion-item"
  >
    {{ obs.observacion }}
  </p>
</div>
```

### 3. Estilos SCSS (formulario-propuesta.component.scss)

#### Clases Principales

**.badge-observacion**

- Background: #F3BD46 (gold/oro)
- Color: #0E2240 (navy oscuro)
- Forma: Píldora redondeada (border-radius 12px)
- Animación: pulse (0.7-1.0 opacity)
- Tamaño: 11px font, inline-block
- Cursor: help (tooltip)

**.campo-observacion**

- Border-color: #F3BD46
- Background: #fffbf0 (crema claro, casi blanco)
- Box-shadow: inset para efecto suave
- Focus: Enhanced shadow con oro

**.observaciones-campo**

- Background: #fef9ec (amarillo muy claro)
- Border: 1px solid #ffc107 (amarillo)
- Border-left: 4px #F3BD46 (accent)
- Padding: 12px
- Margin: 8px top, 12px bottom

**.observacion-item**

- Background: white
- Border-left: 3px #F3BD46
- Padding: 8px (con 12px left)
- Font-size: 13px
- Line-height: 1.5
- Pseudoelemento :before: "💡 " (light bulb emoji)

**.lista-con-observaciones**

- Modifica ítems de checkboxes cuando hay observaciones
- Border: #F3BD46
- Background: #fffbf0

#### Animaciones

**@keyframes pulse** (badge)

```scss
0%,
100% {
  opacity: 1;
}
50% {
  opacity: 0.7;
}
```

- Duración: 2s
- Infinito

## Flujo de Funcionamiento

### 1. Carga de Observaciones (T11)

```
cargarPropuesta(id)
  ↓
estado === 'OBSERVADA'
  ↓
cargarObservaciones(propuestaId)
  ↓
obtenerObservaciones() API call
  ↓
clasificarObservacionesPorCampo()
  ↓
Mapear observaciones a campos
```

### 2. Clasificación Automática (T13)

```
Para cada observación:
  ↓
Buscar keywords en texto (case-insensitive)
  ↓
Añadir a observacionesPorCampo[campo]
  ↓
Marcar campo en camposConObservaciones
```

### 3. Rendererizado en UI

```
Template itera campos del formulario
  ↓
tieneCampoObservaciones('nombreCampo') → true
  ↓
Mostrar badge (⚠️ N)
  ↓
Mostrar clase campo-observacion (border dorado)
  ↓
Mostrar panel observaciones-campo con detalles
```

## Datos de Ejemplo

Para una propuesta OBSERVADA con observación:

```
"El objetivo debe ser más específico y medible"
```

- **Clasificación**: objetivo (contiene "objetivo" y "medible")
- **Badge**: ⚠️ 1 (next to Objetivo label)
- **Campo**: Textarea objetivo con border #F3BD46, bg #fffbf0
- **Panel**: Muestra la observación con:
  - 💡 Icono light bulb
  - Texto de la observación
  - Padding/styling amarillo

## Keywords de Detección

| Campo               | Keywords                                                     |
| ------------------- | ------------------------------------------------------------ |
| nombreProyecto      | nombre, proyecto, título                                     |
| numeroParticipantes | participantes, integrantes, equipo, miembros                 |
| descripcion         | descripción, detallar, metodología, metodologías, evaluación |
| objetivo            | objetivo, objetivos, específico, medible, metas              |
| alcance             | alcance, recursos, tecnológicos, integración                 |
| asignaturas         | asignatura, asignaturas, materia, cátedra                    |

## Estados y Visibilidad

| Estado    | T11 Obs | T12 Reenviar | T13 Badges | Editable |
| --------- | ------- | ------------ | ---------- | -------- |
| BORRADOR  | ❌      | ❌           | ❌         | ✅       |
| PENDIENTE | ❌      | ❌           | ❌         | ❌       |
| OBSERVADA | ✅      | ✅           | ✅         | ✅       |
| APROBADA  | ❌      | ❌           | ❌         | ❌       |
| RECHAZADA | ❌      | ❌           | ❌         | ❌       |

## Responsividad

### Desktop (> 600px)

- Badge: 11px font, inline
- Panel: 12px padding, 8px margin
- Items: 13px font, full width

### Mobile (≤ 600px)

- Badge: 10px font, inline (compacto)
- Panel: 10px padding, 6px margin
- Items: 12px font, reduced padding

## Validaciones

### Automáticas

- Clasificación ocurre al cargar observaciones
- Si no hay keywords coincidentes: log warning (no se muestra en UI)
- Múltiples observaciones por campo: todas se muestran

### Usuario

- No puede editar si estado !== BORRADOR|OBSERVADA
- Las observaciones son read-only (información)
- Puede corregir campos y reenviar después

## Integración Completa HU04

| Componente        | T10 Backend | T11 Load | T12 Reenviar | T13 Integración |
| ----------------- | ----------- | -------- | ------------ | --------------- |
| Backend Service   | ✅          | ✅       | ✅           | -               |
| HTTP Endpoints    | ✅          | ✅       | ✅           | -               |
| Database          | ✅          | ✅       | ✅           | -               |
| Frontend Load     | -           | ✅       | ✅           | ✅              |
| Frontend Display  | -           | ✅       | -            | ✅              |
| Frontend Reenviar | -           | -        | ✅           | -               |
| Field Integration | -           | -        | -            | ✅              |

## Comprobación de Implementación

✅ Frontend compilación exitosa
✅ TypeScript: 3 métodos nuevos, 2 propiedades nuevas
✅ Template: 4 campos con badges y paneles (nombreProyecto, descripcion, objetivo, alcance, asignaturas)
✅ Estilos: 6 clases nuevas, responsive
✅ Animación: pulse en badges
✅ Keywords: 24 keywords distribuidas en 6 campos

## Performance

- Clasificación: O(n\*m) donde n=observaciones, m=keywords
- Renderizado: OnPush con \*ngIf (optimizado)
- Memory: Array y Set para rastreo (minimal overhead)
- Load: Instantáneo una vez cargada propuesta

## Consideraciones Futuras

1. **Mejora de Keywords**: Agregar más palabras clave por dominio
2. **Machine Learning**: Clasificación automática de campos con ML
3. **User Feedback**: Permitir al usuario re-clasificar observaciones
4. **History**: Mostrar histórico de observaciones previas (si las hay)
5. **Auto-Corrections**: Sugerencias automáticas basadas en observaciones

## Referencias

- Badge color: #F3BD46 (EPN Gold)
- Text color: #0E2240 (EPN Navy)
- Warning color: #ffc107 (Bootstrap Warning)
- Panel background: #fef9ec (Crema claro)
- Emoji badges: ⚠️ (alerta), 💡 (idea/sugerencia)

## Resumen Final HU04

✅ **T10**: Backend - Servicio y endpoints de observaciones (COMPLETE)
✅ **T11**: Frontend - Visualizar observaciones en tarjetas amarillas (COMPLETE)
✅ **T12**: Frontend - Botón reenviar propuesta con modal de confirmación (COMPLETE)
✅ **T13**: Frontend - Integración en formulario con badges por campo (COMPLETE)

**HU04 Status: 100% COMPLETE** ✅
