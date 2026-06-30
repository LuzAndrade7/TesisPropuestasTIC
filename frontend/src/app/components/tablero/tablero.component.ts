import { Component, HostListener, OnInit, OnDestroy } from '@angular/core';
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
  mensajeEsError = false;
  estadoActivo: string | null = null;
  mostrarModalEliminar = false;
  propuestaAEliminar: PropuestaResumen | null = null;
  menuAccionesId: number | null = null;
  menuAccionesHaciaArriba = false;
  mostrarModalEnvio = false;
  propuestaAEnviar: PropuestaResumen | null = null;

  // Estados disponibles
  estados = ['BORRADOR', 'PENDIENTE', 'OBSERVADA', 'APROBADA', 'RECHAZADA'];

  // Unsubscribe
  private destroy$ = new Subject<void>();

  constructor(
    private propuestaService: PropuestaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    console.log(' T05: Componente Tablero iniciado');
    this.cargarPropuestas();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Cierra menús flotantes al hacer clic fuera.
   */
  @HostListener('document:click')
  onDocumentClick(): void {
    this.cerrarMenuAcciones();
  }

  /**
   * T05: Cargar propuestas del API
   */
  cargarPropuestas(): void {
    this.cargando = true;
    this.mensaje = '';
    this.mensajeEsError = false;
    
    console.log(' T06: Cargando propuestas del API...');
    
    this.propuestaService.obtenerPropuestas()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (propuestas) => {
          console.log(' T06: Propuestas recibidas:', propuestas.length);
          
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
            this.mensajeEsError = false;
          }
        },
        error: (error) => {
          console.error(' T06: Error cargando propuestas:', error);
          this.mensaje = 'Error al cargar propuestas del servidor';
          this.mensajeEsError = true;
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
    console.log(' T05: Filtrando por estado:', estado || 'TODAS');
    
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
      this.mensajeEsError = false;
    } else {
      this.mensaje = '';
      this.mensajeEsError = false;
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
   * T05: Obtener clase visual del filtro por estado.
   */
  obtenerClaseFiltro(estado: string | null): string {
    if (estado === null) {
      return 'stat-btn--todos';
    }

    return `stat-btn--${estado.toLowerCase()}`;
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
    console.log(' HU06 T19: Ver detalle propuesta:', id);
    this.router.navigate(['/propuestas', id, 'detalle']);
  }

  /**
   * T05: Indica si la propuesta puede editarse segun su estado.
   */
  puedeEditar(estado: string): boolean {
    return estado === 'BORRADOR' || estado === 'OBSERVADA';
  }

  /**
   * HU07: Indica si la propuesta puede asignar estudiantes segun su estado.
   */
  puedeAsignarEstudiantes(estado: string): boolean {
    return estado === 'APROBADA' || estado === 'PENDIENTE';
  }

  /**
   * Indica si una propuesta puede enviarse o reenviarse desde el tablero.
   */
  puedeEnviar(estado: string): boolean {
    return estado === 'BORRADOR' || estado === 'OBSERVADA';
  }

  /**
   * T05: Indica si la propuesta puede eliminarse segun su estado.
   */
  puedeEliminar(estado: string): boolean {
    return estado === 'BORRADOR';
  }

  /**
   * T05: Editar propuesta
   */
  editarPropuesta(id: number): void {
    console.log(' T05: Editar propuesta:', id);
    this.router.navigate(['/propuestas', id, 'editar']);
  }

  /**
   * HU07: Asignar estudiantes a propuesta
   */
  asignarEstudiantes(id: number): void {
    console.log(' HU07: Asignar estudiantes a propuesta:', id);
    this.router.navigate(['/propuestas', id, 'asignar-estudiantes'], {
      queryParams: { returnUrl: '/tablero' }
    });
  }

  /**
   * Abre o cierra el menú de acciones de una propuesta.
   */
  toggleMenuAcciones(id: number, event: Event): void {
    event.stopPropagation();

    if (this.menuAccionesId === id) {
      this.cerrarMenuAcciones();
      return;
    }

    const boton = event.currentTarget as HTMLElement;
    const posicion = boton.getBoundingClientRect();
    const alturaMenuEstimada = 210;
    const espacioInferior = window.innerHeight - posicion.bottom;
    const espacioSuperior = posicion.top;

    this.menuAccionesHaciaArriba =
      espacioInferior < alturaMenuEstimada && espacioSuperior > espacioInferior;
    this.menuAccionesId = id;
  }

  /**
   * Cierra cualquier menú de acciones abierto.
   */
  cerrarMenuAcciones(): void {
    this.menuAccionesId = null;
    this.menuAccionesHaciaArriba = false;
  }

  /**
   * Envía o reenvía una propuesta desde el tablero.
   */
  enviarDesdeTablero(propuesta: PropuestaResumen): void {
    this.cerrarMenuAcciones();
    this.propuestaAEnviar = propuesta;
    this.mostrarModalEnvio = true;
  }

  /**
   * Cierra el modal de envío.
   */
  cancelarEnvio(): void {
    this.mostrarModalEnvio = false;
    this.propuestaAEnviar = null;
  }

  /**
   * Confirma el envío o reenvío desde el modal formal.
   */
  confirmarEnvio(): void {
    const propuesta = this.propuestaAEnviar;
    if (!propuesta) {
      return;
    }

    const esObservada = propuesta.estado === 'OBSERVADA';
    const operacion = esObservada
      ? this.propuestaService.reenviarDespuesDeObservaciones(propuesta.id)
      : this.propuestaService.enviarARevision(propuesta.id);

    this.cancelarEnvio();
    operacion
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.mensaje = esObservada
            ? 'Propuesta reenviada a revisión correctamente'
            : 'Propuesta enviada a revisión correctamente';
          this.mensajeEsError = false;
          this.cargarPropuestas();
        },
        error: (error) => {
          this.mensaje = error?.message || 'No se pudo enviar la propuesta. Revise que los datos estén completos.';
          this.mensajeEsError = true;
        }
      });
  }

  /**
   * T05: Eliminar propuesta
   */
  eliminarPropuesta(id: number): void {
    this.cerrarMenuAcciones();
    const propuesta = this.propuestas.find(item => item.id === id) || null;

    if (!propuesta) {
      return;
    }

    this.propuestaAEliminar = propuesta;
    this.mostrarModalEliminar = true;
  }

  /**
   * Cierra el modal de eliminacion de propuesta.
   */
  cancelarEliminacion(): void {
    this.mostrarModalEliminar = false;
    this.propuestaAEliminar = null;
  }

  /**
   * Confirma la eliminacion definitiva de una propuesta.
   */
  confirmarEliminacion(): void {
    if (!this.propuestaAEliminar) {
      return;
    }

    const id = this.propuestaAEliminar.id;
    console.log(' T05: Eliminando propuesta:', id);

    this.propuestaService.eliminarPropuesta(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log(' T05: Propuesta eliminada');
          this.cancelarEliminacion();
          this.cargarPropuestas();
        },
        error: (error) => {
          console.error(' T05: Error eliminando propuesta:', error);
          this.cancelarEliminacion();
          alert('Error al eliminar la propuesta: ' + (error.message || 'Error desconocido'));
        }
      });
  }

  /**
   * T05: Ir a crear nueva propuesta
   */
  crearNuevaPropuesta(): void {
    console.log(' T05: Ir a crear nueva propuesta');
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
