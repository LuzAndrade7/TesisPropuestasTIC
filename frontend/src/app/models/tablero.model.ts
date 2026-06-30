/**
 * T05: Modelos para el Tablero (HU02)
 * Ubicación: src/app/models/tablero.model.ts
 * 
 * Contiene interfaces para respuestas del API
 * y objetos del tablero
 */

/**
 * Información resumida de una propuesta para el tablero
 */
export interface PropuestaResumen {
  id: number;
  nombreProyecto: string;
  estado: 'BORRADOR' | 'PENDIENTE' | 'OBSERVADA' | 'APROBADA' | 'RECHAZADA';
  numeroParticipantes: number;
  fechaActualizacion: Date;
  fechaCreacion: Date;
  fechaEnvioRevision?: Date;
  profesor?: DocenteResumen;
  asignaturas?: Asignatura[];
}

/**
 * Información resumida de docente
 */
export interface DocenteResumen {
  id: number;
  nombreCompleto: string;
  correo?: string;
}

/**
 * Asignatura
 */
export interface Asignatura {
  id: number;
  codigo: string;
  nombre: string;
}

/**
 * Estadísticas del tablero (conteos por estado)
 */
export interface EstadisticasTablero {
  total: number;
  borrador: number;
  pendiente: number;
  observada: number;
  aprobada: number;
  rechazada: number;
}

/**
 * Filtro para tablero
 */
export interface FiltroTablero {
  estado?: string; // null = todos, o un estado específico
}
