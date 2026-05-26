import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { PropuestaService } from '../../services/propuesta.service';
import { PropuestaResumen, EstadisticasTablero } from '../../models/tablero.model';

/**
 * T05: Componente Tablero - Visualizar propuestas (HU02)
 * Ubicación: src/app/components/tablero/tablero.component.ts
 * 
 * Funcionalidades:
 * - Mostrar tabla de propuestas
 * - Filtrar por estado
 * - Mostrar badges con colores por estado
 * - Botones para ver/editar/eliminar
 * - Estadísticas por estado
 */
@Component({
  selector: 'app-tablero',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tablero.component.html',
  styleUrls: ['./tablero.component.scss']
})
export class TableroComponent implements OnInit, OnDestroy {
  // Datos
  propuestas: PropuestaResumen[] = [];
  propuestasFiltradas: PropuestaResumen[] = [];
  estadisticas: EstadisticasTablero = {
    total: 0,
    borrador: 0,
    pendiente: 0,
    observada: 0,
    aprobada: 0,
    rechazada: 0
  };

  // Control UI
  cargando = false;
  mensaje = '';
  estadoActivo: string | null = null;

  // Estados disponibles
  estados = ['BORRADOR', 'PENDIENTE', 'OBSERVADA', 'APROBADA', 'RECHAZADA'];

  // Unsubscribe
  private destroy$ = new Subject<void>();

  constructor(
    private propuestaService: PropuestaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    console.log('✅ T05: Componente Tablero iniciado');
    this.cargarPropuestas();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * T05: Cargar propuestas del API
   */
  cargarPropuestas(): void {
    this.cargando = true;
    this.mensaje = '';
    
    console.log('✅ T06: Cargando propuestas del API...');
    
    this.propuestaService.obtenerPropuestas()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (propuestas) => {
          console.log('✅ T06: Propuestas recibidas:', propuestas.length);
          
          // Asegurar que las fechas sean Date objects
          this.propuestas = propuestas.map(p => ({
            ...p,
            fechaActualizacion: new Date(p.fechaActualizacion),
            fechaCreacion: new Date(p.fechaCreacion),
            fechaEnvioRevision: p.fechaEnvioRevision ? new Date(p.fechaEnvioRevision) : undefined
          }));

          // Mostrar todo por defecto
          this.propuestasFiltradas = [...this.propuestas];
          this.calcularEstadisticas();
          this.cargando = false;

          if (this.propuestas.length === 0) {
            this.mensaje = 'No hay propuestas aún. Crea una nueva propuesta para comenzar.';
          }
        },
        error: (error) => {
          console.error('❌ T06: Error cargando propuestas:', error);
          this.mensaje = 'Error al cargar propuestas del servidor';
          this.cargando = false;
        }
      });
  }

  /**
   * T05: Calcular estadísticas por estado
   */
  private calcularEstadisticas(): void {
    this.estadisticas = {
      total: this.propuestas.length,
      borrador: this.propuestas.filter(p => p.estado === 'BORRADOR').length,
      pendiente: this.propuestas.filter(p => p.estado === 'PENDIENTE').length,
      observada: this.propuestas.filter(p => p.estado === 'OBSERVADA').length,
      aprobada: this.propuestas.filter(p => p.estado === 'APROBADA').length,
      rechazada: this.propuestas.filter(p => p.estado === 'RECHAZADA').length
    };
  }

  /**
   * T05: Filtrar propuestas por estado
   * Si estado es null, mostrar todas
   */
  filtrarPorEstado(estado: string | null): void {
    console.log('✅ T05: Filtrando por estado:', estado || 'TODAS');
    
    this.estadoActivo = estado;
    
    if (estado === null) {
      // Mostrar todas
      this.propuestasFiltradas = [...this.propuestas];
    } else {
      // Filtrar
      this.propuestasFiltradas = this.propuestas.filter(p => p.estado === estado);
    }

    if (this.propuestasFiltradas.length === 0 && estado) {
      this.mensaje = `No hay propuestas con estado "${estado}"`;
    } else {
      this.mensaje = '';
    }
  }

  /**
   * T05: Obtener clase de badge por estado
   */
  obtenerClaseBadge(estado: string): string {
    switch (estado) {
      case 'BORRADOR': return 'badge-borrador';
      case 'PENDIENTE': return 'badge-pendiente';
      case 'OBSERVADA': return 'badge-observada';
      case 'APROBADA': return 'badge-aprobada';
      case 'RECHAZADA': return 'badge-rechazada';
      default: return '';
    }
  }

  /**
   * T05: Obtener etiqueta legible del estado
   */
  obtenerEtiquetaEstado(estado: string): string {
    switch (estado) {
      case 'BORRADOR': return 'Borrador';
      case 'PENDIENTE': return 'Pendiente';
      case 'OBSERVADA': return 'Observada';
      case 'APROBADA': return 'Aprobada';
      case 'RECHAZADA': return 'Rechazada';
      default: return estado;
    }
  }

  /**
   * T05: Obtener número en botón estadísticas
   */
  obtenerNumeroEstado(estado: string): number {
    switch (estado) {
      case 'BORRADOR': return this.estadisticas.borrador;
      case 'PENDIENTE': return this.estadisticas.pendiente;
      case 'OBSERVADA': return this.estadisticas.observada;
      case 'APROBADA': return this.estadisticas.aprobada;
      case 'RECHAZADA': return this.estadisticas.rechazada;
      default: return 0;
    }
  }

  /**
   * T05: Ver detalle de propuesta
   * HU06 T19: Navega a la vista detalle completo
   */
  verDetalle(id: number): void {
    console.log('✅ HU06 T19: Ver detalle propuesta:', id);
    this.router.navigate(['/propuestas', id, 'detalle']);
  }

  /**
   * T05: Editar propuesta
   */
  editarPropuesta(id: number): void {
    console.log('✅ T05: Editar propuesta:', id);
    this.router.navigate(['/propuestas', id, 'editar']);
  }

  /**
   * T05: Eliminar propuesta
   */
  eliminarPropuesta(id: number): void {
    if (!confirm('¿Estás seguro que deseas eliminar esta propuesta?')) {
      return;
    }

    console.log('✅ T05: Eliminando propuesta:', id);
    
    this.propuestaService.eliminarPropuesta(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log('✅ T05: Propuesta eliminada');
          // Recargar tabla
          this.cargarPropuestas();
        },
        error: (error) => {
          console.error('❌ T05: Error eliminando propuesta:', error);
          alert('Error al eliminar la propuesta: ' + (error.message || 'Error desconocido'));
        }
      });
  }

  /**
   * T05: Ir a crear nueva propuesta
   */
  crearNuevaPropuesta(): void {
    console.log('✅ T05: Ir a crear nueva propuesta');
    this.router.navigate(['/propuestas/nueva']);
  }

  /**
   * T05: Formatear fecha para mostrar
   */
  formatearFecha(fecha: Date): string {
    if (!fecha) return '';
    const d = new Date(fecha);
    const dia = String(d.getDate()).padStart(2, '0');
    const mes = String(d.getMonth() + 1).padStart(2, '0');
    const anio = d.getFullYear();
    return `${dia}/${mes}/${anio}`;
  }
}
