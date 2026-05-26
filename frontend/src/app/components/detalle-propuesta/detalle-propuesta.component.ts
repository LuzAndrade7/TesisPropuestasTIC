import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PropuestaService } from '../../services/propuesta.service';
import { AsignarEstudiantesComponent } from '../asignar-estudiantes/asignar-estudiantes.component';

/**
 * HU06 T18 + HU07 T24-T25: Componente para visualizar detalle completo de propuesta
 * Muestra: datos básicos, profesor, asignaturas, observaciones, histórico de estados
 * Botones contextuales según estado
 */
@Component({
  selector: 'app-detalle-propuesta',
  standalone: true,
  imports: [CommonModule, AsignarEstudiantesComponent],
  templateUrl: './detalle-propuesta.component.html',
  styleUrls: ['./detalle-propuesta.component.scss']
})
export class DetallePropuestaComponent implements OnInit, OnDestroy {
  propuestaId: number | null = null;
  detalle: any = null;
  
  // Estados de carga
  cargando = true;
  error: string | null = null;
  
  // HU07 T24-T25: Control del modal de asignación
  mostrarModalAsignarEstudiantes = false;
  estudiantesAsignados: any[] = [];
  
  // Colores para estados
  estadoColores: { [key: string]: string } = {
    'BORRADOR': '#0E2240',      // Navy
    'PENDIENTE': '#77A4DC',     // Blue
    'OBSERVADA': '#F3BD46',     // Gold
    'APROBADA': '#28a745',      // Green
    'RECHAZADA': '#E31D1A'      // Red
  };

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private propuestaService: PropuestaService
  ) {}

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      this.propuestaId = params['id'];
      if (this.propuestaId) {
        this.cargarDetalleCompleto(this.propuestaId);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Carga el detalle completo de la propuesta desde el API
   */
  private cargarDetalleCompleto(id: number): void {
    this.cargando = true;
    this.error = null;
    
    this.propuestaService.obtenerDetalleCompleto(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (data) => {
          this.detalle = data;
          this.cargando = false;
          this.cargarEstudiantesAsignados(id);
        },
        (error) => {
          this.error = error?.message || 'Error al cargar detalle de la propuesta';
          this.cargando = false;
          console.error('Error loading detalle:', error);
        }
      );
  }

  /**
   * HU07 T24: Carga estudiantes ya asignados a la propuesta
   */
  private cargarEstudiantesAsignados(propuestaId: number): void {
    this.propuestaService.obtenerEstudiantesAsignados(propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        (estudiantes: any[]) => {
          this.estudiantesAsignados = estudiantes;
        },
        (error) => {
          console.error('Error cargando estudiantes asignados:', error);
        }
      );
  }

  /**
   * Obtiene el color para un estado específico
   */
  getColorEstado(estado: string): string {
    return this.estadoColores[estado] || '#0E2240';
  }

  /**
   * Verifica si se puede editar la propuesta
   */
  get puedeEditar(): boolean {
    return this.detalle?.puedeEditar === true;
  }

  /**
   * Verifica si se puede reenviar la propuesta
   */
  get puedeReenviar(): boolean {
    return this.detalle?.puedeReenviar === true;
  }

  /**
   * Verifica si se puede eliminar la propuesta
   */
  get puedeEliminar(): boolean {
    return this.detalle?.puedeEliminar === true;
  }

  /**
   * HU07 T24: Verifica si se puede asignar estudiantes
   * Solo APROBADA o PENDIENTE
   */
  get puedeAsignarEstudiantes(): boolean {
    return this.detalle?.estado === 'APROBADA' || this.detalle?.estado === 'PENDIENTE';
  }

  /**
   * HU07 T25: Verifica si se puede solicitar nueva aprobación
   * Solo APROBADA
   */
  get puedeSolicitarNuevaAprobacion(): boolean {
    return this.detalle?.estado === 'APROBADA';
  }

  /**
   * Navega a la página de edición
   */
  irAEditar(): void {
    if (this.propuestaId) {
      this.router.navigate(['/propuestas', this.propuestaId]);
    }
  }

  /**
   * Abre modal de confirmación para envío a revisión
   */
  enviarARevision(): void {
    if (confirm('¿Deseas enviar esta propuesta a revisión?\n\nUna vez enviada, no podrás editarla hasta que recibas observaciones.')) {
      // TODO: Implementar envío a revisión en el servicio
      console.log('Enviar a revisión:', this.propuestaId);
    }
  }

  /**
   * Abre modal de confirmación para reenvío
   */
  reenviarPropuesta(): void {
    if (confirm('¿Deseas reenviar esta propuesta corregida?\n\nSe limpiarán todas las observaciones y se marcará como PENDIENTE.')) {
      // TODO: Implementar reenvío en el servicio
      console.log('Reenviar propuesta:', this.propuestaId);
    }
  }

  /**
   * HU08 T26-T28: Elimina propuesta en estado BORRADOR
   * Solicita confirmación antes de ejecutar
   * Navega a tablero después de exitoso
   */
  eliminarPropuesta(): void {
    if (!this.propuestaId) return;

    // Solo permitir eliminación de propuestas BORRADOR
    if (this.detalle?.estado !== 'BORRADOR') {
      alert('⚠️ Error: Solo se pueden eliminar propuestas en estado BORRADOR.\n\nEstado actual: ' + this.detalle?.estado);
      return;
    }

    const nombrePropuesta = this.detalle?.titulo || 'Sin título';
    const confirmacion = confirm(
      `🗑️ ELIMINAR PROPUESTA\n\n` +
      `Propuesta: ${nombrePropuesta}\n` +
      `Estado: BORRADOR\n\n` +
      `⚠️ ADVERTENCIA: Esta acción NO se puede deshacer.\n\n` +
      `¿Deseas continuar?`
    );

    if (!confirmacion) {
      return; // Usuario canceló
    }

    // Llamar al servicio para eliminar
    this.propuestaService.eliminarPropuesta(this.propuestaId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (respuesta: any) => {
          console.log('✅ HU08 T28: Propuesta eliminada correctamente', respuesta);
          alert('✓ Propuesta eliminada exitosamente.\n\nSerás redirigido al tablero.');
          
          // Navegar al tablero
          setTimeout(() => {
            this.router.navigate(['/tablero']);
          }, 500);
        },
        error: (error: any) => {
          console.error('❌ HU08 T28: Error al eliminar propuesta', error);
          
          // Manejo de errores específicos
          if (error?.status === 403) {
            alert('⚠️ Error: Solo se pueden eliminar propuestas en estado BORRADOR.');
          } else if (error?.status === 404) {
            alert('⚠️ Error: Propuesta no encontrada.');
          } else if (error?.status === 400) {
            alert('⚠️ Error: Datos inválidos.');
          } else {
            const mensaje = error?.error?.message || 'Error al eliminar la propuesta';
            alert('❌ Error: ' + mensaje);
          }
        }
      });
  }

  /**
   * HU07 T24: Abre modal para asignar estudiantes
   */
  abrirModalAsignarEstudiantes(): void {
    this.mostrarModalAsignarEstudiantes = true;
  }

  /**
   * HU07 T24: Cierra modal de asignación
   */
  cerrarModalAsignarEstudiantes(): void {
    this.mostrarModalAsignarEstudiantes = false;
  }

  /**
   * HU07 T24-T25: Manejador cuando se completa la asignación
   */
  onAsignacionCompleta(evento: any): void {
    console.log('✅ HU07: Asignación completada', evento);
    this.cerrarModalAsignarEstudiantes();
    
    // Si hubo cambios en APROBADA, solicitar nueva aprobación automáticamente
    if (evento.cambioEstado && this.propuestaId) {
      setTimeout(() => {
        this.solicitarNuevaAprobacion();
      }, 1000);
    } else {
      // Recargar la página para ver los cambios
      if (this.propuestaId) {
        this.cargarDetalleCompleto(this.propuestaId);
      }
    }
  }

  /**
   * HU07 T25: Solicita nueva aprobación para propuesta APROBADA
   * Cambia estado APROBADA → PENDIENTE
   */
  solicitarNuevaAprobacion(): void {
    if (!this.propuestaId) return;

    const motivo = prompt(
      '⚠️ Cambios Detectados\n\n' +
      'Esta propuesta está en estado APROBADA. Al solicitar nueva aprobación, pasará a PENDIENTE.\n\n' +
      'Por favor, indica el motivo de la solicitud:'
    );

    if (!motivo || !motivo.trim()) {
      return; // Usuario canceló
    }

    this.propuestaService.solicitarNuevaAprobacion(this.propuestaId, motivo.trim())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (propuestaActualizada: any) => {
          console.log('✅ HU07 T25: Nueva aprobación solicitada');
          // Actualizar el detalle local
          this.detalle = propuestaActualizada;
          alert('✓ Nueva aprobación solicitada correctamente.\n\nLa propuesta cambió a estado PENDIENTE.');
          
          // Recargar después de 1 segundo
          setTimeout(() => {
            if (this.propuestaId) {
              this.cargarDetalleCompleto(this.propuestaId);
            }
          }, 1000);
        },
        error: (error: any) => {
          console.error('Error solicitando nueva aprobación:', error);
          alert('❌ Error: ' + (error?.message || 'No se pudo solicitar nueva aprobación'));
        }
      });
  }

  /**
   * Vuelve al tablero
   */
  volverAlTablero(): void {
    this.router.navigate(['/tablero']);
  }

  /**
   * Formatea una fecha para mostrar
   */
  formatearFecha(fecha: string | Date): string {
    if (!fecha) return '-';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  /**
   * Obtiene el texto descriptivo de un estado anterior nulo
   */
  getEstadoAnterior(estadoAnterior: string | null): string {
    if (!estadoAnterior) return 'Creación';
    return estadoAnterior;
  }
}
