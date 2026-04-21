import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

export interface Propuesta {
  id?: number;
  titulo: string;
  proyecto: string;
  descripcion: string;
  objetivo: string;
  alcance: string;
  numeroParticipantes: number;
  asignaturas: string[];
  modulos: any[];
  estado?: string;
  fecha?: string;
}

export interface ModuloDetalle {
  id: number;
  numeroModulo: number;
  nombre: string;
  descripcion: string;
  productos: string;
  estudianteAsignado: string;
  actividades: any[];
}

@Injectable({
  providedIn: 'root'
})
export class PropuestaService {
  private apiUrl = 'http://localhost:5000/api/propuestas';
  private propuestasSubject = new BehaviorSubject<any[]>([]);
  
  constructor(private http: HttpClient) {}

  // Dashboard - Obtener todas las propuestas
  obtenerPropuestas(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // Dashboard - Obtener estadísticas (calculamos en cliente)
  obtenerEstadisticas(): Observable<any> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map((props: any[]) => ({
        todos: props.length,
        borrador: props.filter(p => p.estado === 'Borrador').length,
        pendiente: props.filter(p => p.estado === 'Pendiente').length,
        observada: props.filter(p => p.estado === 'Observada').length,
        aprobada: props.filter(p => p.estado === 'Aprobada').length,
        rechazada: props.filter(p => p.estado === 'Rechazada').length
      }))
    );
  }

  // Detalle - Obtener propuesta por ID
  obtenerPropuesta(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  // Crear nueva propuesta
  crearPropuesta(propuesta: Propuesta): Observable<any> {
    return this.http.post<any>(this.apiUrl, propuesta);
  }

  // Actualizar propuesta
  actualizarPropuesta(id: number, propuesta: Propuesta): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, propuesta);
  }

  // Eliminar propuesta
  eliminarPropuesta(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  // Cambiar estado
  cambiarEstado(id: number, estado: string): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/cambiar-estado`, { estado });
  }

  // Asignar estudiantes
  asignarEstudiantes(id: number, modulos: any[]): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${id}/asignar-estudiantes`, { modulos });
  }

  // Obtener por estado
  obtenerPorEstado(estado: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/estado/${estado}`);
  }
}
