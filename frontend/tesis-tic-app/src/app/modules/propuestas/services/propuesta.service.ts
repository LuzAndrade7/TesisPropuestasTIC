import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  Propuesta,
  CrearPropuestaRequest,
  ActualizarPropuestaRequest,
  CambiarEstadoRequest,
  Estado,
  DocenteInfo,
  LineaInvestigacion,
  Asignatura
} from '../models/propuesta.model';

@Injectable({
  providedIn: 'root'
})
export class PropuestaService {
  private apiUrl = '/api';

  constructor(private http: HttpClient) {}

  crearPropuesta(request: CrearPropuestaRequest): Observable<Propuesta> {
    return this.http.post<Propuesta>(`${this.apiUrl}/propuestas`, request);
  }

  obtenerPropuesta(id: number): Observable<Propuesta> {
    return this.http.get<Propuesta>(`${this.apiUrl}/propuestas/${id}`);
  }

  obtenerTodasPropuestas(): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(`${this.apiUrl}/propuestas`);
  }

  obtenerPropuestasPorDocente(docenteId: number): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(
      `${this.apiUrl}/propuestas/docente/${docenteId}`
    );
  }

  obtenerPropuestasPorEstado(estadoId: number): Observable<Propuesta[]> {
    return this.http.get<Propuesta[]>(
      `${this.apiUrl}/propuestas/estado/${estadoId}`
    );
  }

  actualizarPropuesta(request: ActualizarPropuestaRequest): Observable<Propuesta> {
    return this.http.put<Propuesta>(
      `${this.apiUrl}/propuestas/${request.id}`,
      request
    );
  }

  cambiarEstado(request: CambiarEstadoRequest): Observable<{ message: string }> {
    return this.http.patch<{ message: string }>(
      `${this.apiUrl}/propuestas/${request.propuestaId}/estado`,
      request
    );
  }

  asignarEstudiante(propuestaId: number, estudianteId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/propuestas/${propuestaId}/estudiantes`,
      { propuestaId, estudianteId }
    );
  }

  eliminarPropuesta(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/propuestas/${id}`
    );
  }

  obtenerEstados(): Observable<Estado[]> {
    return this.http.get<Estado[]>(`${this.apiUrl}/estados`);
  }

  obtenerDocentes(): Observable<DocenteInfo[]> {
    return this.http.get<DocenteInfo[]>(`${this.apiUrl}/docentes`);
  }

  obtenerDocente(id: number): Observable<DocenteInfo> {
    return this.http.get<DocenteInfo>(`${this.apiUrl}/docentes/${id}`);
  }

  obtenerAsignaturas(): Observable<Asignatura[]> {
    return this.http.get<Asignatura[]>(`${this.apiUrl}/asignaturas`);
  }
}
