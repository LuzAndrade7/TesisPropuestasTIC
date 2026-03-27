export interface Estado {
  id: number;
  nombre: string;
  descripcion: string;
}

export interface DocenteInfo {
  id: number;
  nombre: string;
  apellido: string;
  correoInstitucional: string;
  departamento: string;
  activo: boolean;
}

export interface LineaInvestigacion {
  id: number;
  nombre: string;
  descripcion: string;
}

export interface Asignatura {
  id: number;
  nombre: string;
  codigo: string;
}

export interface EstudianteAsignado {
  id: number;
  estudianteId: number;
  nombreEstudiante: string;
  apellidoEstudiante: string;
  matriculaEstudiante: string;
  fechaAsignacion: Date;
}

export interface Propuesta {
  id: number;
  titulo: string;
  descripcion: string;
  objetivo: string;
  alcance: string;
  componentesActividadesProductos: string;
  docenteId: number;
  docenteNombre: string;
  estadoId: number;
  estadoNombre: string;
  lineaInvestigacionId: number;
  lineaInvestigacionNombre: string;
  observaciones: string;
  numeroParticipantes: number;
  departamento: string;
  facultad: string;
  fechaCreacion: Date;
  fechaActualizacion: Date;
  fechaEnvioPrimera: Date;
  estudiantesAsignados: EstudianteAsignado[];
  asignaturas: Asignatura[];
}

export interface CrearPropuestaRequest {
  titulo: string;
  descripcion: string;
  objetivo: string;
  alcance: string;
  componentesActividadesProductos: string;
  docenteId: number;
  lineaInvestigacionId: number | null;
  numeroParticipantes: number;
  departamento: string;
  facultad: string;
  asignaturasIds: number[];
}

export interface ActualizarPropuestaRequest {
  id: number;
  titulo: string;
  descripcion: string;
  objetivo: string;
  alcance: string;
  componentesActividadesProductos: string;
  lineaInvestigacionId: number | null;
  observaciones: string;
  numeroParticipantes: number;
  departamento: string;
  facultad: string;
  asignaturasIds: number[];
}

export interface CambiarEstadoRequest {
  propuestaId: number;
  estadoId: number;
  observaciones: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  message: string;
}

export interface ApiResponse<T> {
  data: T;
  message: string;
}
