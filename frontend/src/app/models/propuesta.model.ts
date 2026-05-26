/**
 * Modelos y Interfaces TypeScript para propuestas
 * Mapean los DTOs del backend
 */

/**
 * Interface para Asignatura
 */
export interface Asignatura {
  id: number;
  codigo: string;
  nombre: string;
}

/**
 * Interface para Docente
 */
export interface Docente {
  id: number;
  nombres: string;
  apellidos: string;
  correo?: string;
  tituloAcademico?: string;
}

/**
 * Interface para Propuesta (lectura)
 * Usada cuando se obtiene una propuesta del backend
 */
export interface Propuesta {
  id: number;
  nombreProyecto: string;
  numeroParticipantes: number;
  profesorId: number;
  descripcion: string;
  objetivo: string;
  alcance: string;
  estado: 'BORRADOR' | 'PENDIENTE' | 'OBSERVADA' | 'APROBADA' | 'RECHAZADA';
  fechaCreacion: Date;
  fechaActualizacion: Date;
  fechaEnvioRevision?: Date;
  profesor?: Docente;
  asignaturas: Asignatura[];
}

/**
 * Interface para crear propuesta (POST)
 * Usada al enviar el formulario
 */
export interface CreatePropuestaRequest {
  nombreProyecto: string;
  numeroParticipantes: number;
  profesorId: number;
  descripcion: string;
  objetivo: string;
  alcance: string;
  asignaturaIds?: number[];
}

/**
 * Interface para actualizar propuesta (PUT)
 */
export interface UpdatePropuestaRequest {
  nombreProyecto?: string;
  numeroParticipantes?: number;
  descripcion?: string;
  objetivo?: string;
  alcance?: string;
  asignaturaIds?: number[];
}

/**
 * Interface para cambiar estado
 */
export interface CambiarEstadoRequest {
  estado: 'BORRADOR' | 'PENDIENTE' | 'OBSERVADA' | 'APROBADA' | 'RECHAZADA';
}

/**
 * Interface para respuesta de error del API
 */
export interface ErrorResponse {
  message: string;
  statusCode?: number;
}
