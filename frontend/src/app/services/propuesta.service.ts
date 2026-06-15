import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  Propuesta,
  CreatePropuestaRequest,
  UpdatePropuestaRequest,
  CambiarEstadoRequest,
  Asignatura,
  Docente,
  ErrorResponse
} from '../models/propuesta.model';
import { PropuestaResumen } from '../models/tablero.model';

/**
 * T03: Servicio HTTP para operaciones de propuestas
 * Maneja la comunicación con el backend API
 * Ubicación: src/app/services/propuesta.service.ts
 */
@Injectable({
  providedIn: 'root'
})
export class PropuestaService {
  private apiUrl = `${environment.apiUrl}/propuestas`;
  private asignaturasUrl = `${environment.apiUrl}/asignaturas`;
  private docentesUrl = `${environment.apiUrl}/docentes`;

  constructor(private http: HttpClient) {}

  /**
   * T06: Obtener todas las propuestas para el tablero (HU02)
   * Con información resumida
   */
  obtenerPropuestas(estado?: string): Observable<PropuestaResumen[]> {
    let url = this.apiUrl;
    if (estado) {
      url += `?estado=${estado}`;
    }
    return this.http.get<PropuestaResumen[]>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T03: Obtener todas las propuestas
   */
  obtenerPropuestasCompletas(): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T03: Obtener propuesta por ID
   * @param id ID de la propuesta
   */
  obtenerPropuestaById(id: number): Observable<Propuesta> {
    return this.http.get<Propuesta>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU06 T17: Obtener detalle completo de propuesta
   * Incluye: datos básicos, profesor, asignaturas, observaciones, histórico
   * Endpoint: GET /api/propuestas/{id}/detalle
   * @param id ID de la propuesta
   */
  obtenerDetalleCompleto(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}/detalle`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T01: Crear nueva propuesta (guardar como borrador)
   * Endpoint: POST /api/propuestas
   * @param propuesta Datos de la propuesta
   */
  crearPropuesta(propuesta: CreatePropuestaRequest): Observable<Propuesta> {
    console.log(' T01: Creando propuesta:', propuesta);
    return this.http.post<Propuesta>(this.apiUrl, propuesta)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T01: Guardar propuesta como borrador (sin enviar a revisión)
   * Similar a crear pero usado desde el formulario existente
   */
  guardarBorrador(propuesta: CreatePropuestaRequest): Observable<Propuesta> {
    console.log(' T01: Guardando borrador:', propuesta);
    return this.crearPropuesta(propuesta);
  }

  /**
   * T01: Actualizar propuesta existente
   */
  actualizarPropuesta(id: number, propuesta: UpdatePropuestaRequest): Observable<Propuesta> {
    console.log(' T01: Actualizando propuesta', id, ':', propuesta);
    return this.http.put<Propuesta>(`${this.apiUrl}/${id}`, propuesta)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T03: Cambiar estado de propuesta a PENDIENTE (enviar a revisión)
   */
  cambiarEstado(id: number, estado: string): Observable<Propuesta> {
    const request: CambiarEstadoRequest = { 
      estado: estado as any 
    };
    return this.http.patch<Propuesta>(`${this.apiUrl}/${id}/estado`, request)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU03 T07: Enviar propuesta a revisión con validaciones completas
   * Endpoint: POST /api/propuestas/{id}/enviar-revision
   * Valida que todos los campos requeridos estén completos:
   * - Nombre proyecto: 10-250 caracteres
   * - Número participantes: 1-5
   * - Descripción: mínimo 20 caracteres
   * - Objetivo: no vacío
   * - Alcance: no vacío
   * - Mínimo 1 asignatura
   */
  enviarARevision(id: number): Observable<Propuesta> {
    console.log(' HU03 T08: Enviando propuesta', id, 'a revisión');
    return this.http.post<Propuesta>(`${this.apiUrl}/${id}/enviar-revision`, {})
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T03: Eliminar propuesta
   */
  eliminarPropuesta(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T02: Obtener todas las asignaturas para el dropdown
   */
  obtenerAsignaturas(): Observable<Asignatura[]> {
    return this.http.get<Asignatura[]>(this.asignaturasUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * T02: Obtener todos los docentes
   */
  obtenerDocentes(): Observable<Docente[]> {
    return this.http.get<Docente[]>(this.docentesUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Manejo centralizado de errores HTTP
   */
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Error desconocido';

    if (error.error instanceof ErrorEvent) {
      // Error del lado del cliente
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Error del lado del servidor
      errorMessage = error.error?.message || 
                    `Error ${error.status}: ${error.statusText}`;
    }

    console.error(' Error en propuesta.service:', errorMessage);
    return throwError(() => ({
      message: errorMessage,
      statusCode: error.status
    }));
  }

  /**
   * HU04 T11: Obtener observaciones de una propuesta
   */
  obtenerObservaciones(propuestaId: number): Observable<any[]> {
    console.log(' HU04 T11: Obteniendo observaciones para propuesta', propuestaId);
    return this.http.get<any[]>(`${environment.apiUrl}/observaciones/propuesta/${propuestaId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU04 T12: Reenviar propuesta después de correcciones
   * Limpia observaciones y cambia estado OBSERVADA a PENDIENTE
   */
  reenviarDespuesDeObservaciones(id: number): Observable<Propuesta> {
    console.log(' HU04 T12: Reenviando propuesta', id);
    return this.http.post<Propuesta>(`${this.apiUrl}/${id}/reenviar-despues-observaciones`, {})
      .pipe(
        catchError(this.handleError)
      );
  }

  // ===== HU07 T21-T25: MÉTODOS DE ASIGNACIÓN DE ESTUDIANTES =====

  /**
   * HU07 T22: Obtener todos los estudiantes disponibles
   * Endpoint: GET /api/estudiantes
   */
  obtenerTodosEstudiantes(): Observable<any[]> {
    console.log(' HU07 T22: Obteniendo todos los estudiantes');
    return this.http.get<any[]>(`${environment.apiUrl}/estudiantes`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07 T22: Buscar estudiantes por nombre, apellido o correo
   * Endpoint: GET /api/estudiantes/buscar?searchTerm=
   */
  buscarEstudiantes(termino: string): Observable<any[]> {
    console.log(' HU07 T22: Buscando estudiantes:', termino);
    return this.http.get<any[]>(`${environment.apiUrl}/estudiantes/buscar`, {
      params: { searchTerm: termino }
    })
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07 T22: Obtener estudiantes disponibles (no asignados a una propuesta)
   * Endpoint: GET /api/estudiantes/disponibles/{propuestaId}
   */
  obtenerEstudiantesDisponibles(propuestaId: number): Observable<any[]> {
    console.log(' HU07 T22: Obteniendo estudiantes disponibles para propuesta', propuestaId);
    return this.http.get<any[]>(`${environment.apiUrl}/estudiantes/disponibles/${propuestaId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07 T22: Obtener estudiantes ya asignados a una propuesta
   * Endpoint: GET /api/estudiantes/asignados/{propuestaId}
   */
  obtenerEstudiantesAsignados(propuestaId: number): Observable<any[]> {
    console.log(' HU07 T22: Obteniendo estudiantes asignados para propuesta', propuestaId);
    return this.http.get<any[]>(`${environment.apiUrl}/estudiantes/asignados/${propuestaId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07 T20-T22: Asignar estudiantes a una propuesta
   * Endpoint: POST /api/estudiantes/{propuestaId}/asignar
   * 
   * Validaciones:
   * - Máximo 5 estudiantes
   * - Solo APROBADA o PENDIENTE pueden asignar
   * - Si cambios desde APROBADA a PENDIENTE
   */
  asignarEstudiantes(propuestaId: number, estudianteIds: number[], motivo: string): Observable<any[]> {
    console.log(' HU07 T20: Asignando estudiantes a propuesta', propuestaId);
    const payload = {
      estudianteIds: estudianteIds,
      motivo: motivo,
      realizadoPor: 'usuario@example.com'
    };
    return this.http.post<any[]>(`${environment.apiUrl}/estudiantes/${propuestaId}/asignar`, payload)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07: Asigna estudiantes creados desde nombres escritos en el formulario.
   */
  asignarEstudiantesPorNombre(propuestaId: number, nombresEstudiante: string[], motivo: string): Observable<any[]> {
    const payload = {
      nombresEstudiante,
      motivo,
      realizadoPor: 'usuario@example.com'
    };

    return this.http.post<any[]>(`${environment.apiUrl}/estudiantes/${propuestaId}/asignar`, payload)
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * HU07 T25: Solicitar nueva aprobación para propuesta APROBADA
   * Endpoint: POST /api/propuestas/{id}/solicitar-nueva-aprobacion
   * 
   * Efecto: APROBADA a PENDIENTE
   */
  solicitarNuevaAprobacion(propuestaId: number, motivo: string): Observable<Propuesta> {
    console.log(' HU07 T25: Solicitando nueva aprobación para propuesta', propuestaId);
    const payload = { motivo: motivo };
    return this.http.post<Propuesta>(`${this.apiUrl}/${propuestaId}/solicitar-nueva-aprobacion`, payload)
      .pipe(
        catchError(this.handleError)
      );
  }
}
