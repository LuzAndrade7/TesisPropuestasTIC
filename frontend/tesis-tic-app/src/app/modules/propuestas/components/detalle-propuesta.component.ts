import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PropuestaService } from '../services/propuesta.service';
import { Propuesta, Estado, CambiarEstadoRequest } from '../../shared/models/propuesta.model';

@Component({
  selector: 'app-detalle-propuesta',
  templateUrl: './detalle-propuesta.component.html',
  styleUrls: ['./detalle-propuesta.component.css']
})
export class DetallePropuestaComponent implements OnInit, OnDestroy {
  propuesta: Propuesta | null = null;
  estados: Estado[] = [];
  cargando: boolean = true;
  error: string = '';
  exito: string = '';
  confirmandoCambioEstado: boolean = false;
  nuevoEstadoSeleccionado: number | null = null;
  observacionesCambio: string = '';

  private destroy$ = new Subject<void>();

  constructor(
    private propuestaService: PropuestaService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarEstados();
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params['id']) {
        this.cargarPropuesta(params['id']);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  cargarEstados(): void {
    this.propuestaService.obtenerEstados().subscribe({
      next: (estados) => this.estados = estados,
      error: () => console.error('Error al cargar estados')
    });
  }

  cargarPropuesta(id: number): void {
    this.cargando = true;
    this.error = '';
    this.propuestaService.obtenerPropuesta(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (propuesta) => {
          this.propuesta = propuesta;
          this.cargando = false;
        },
        error: (error) => {
          this.error = 'Error al cargar la propuesta';
          this.cargando = false;
        }
      });
  }

  mostrarCambioEstado(): void {
    this.confirmandoCambioEstado = true;
  }

  cancelarCambioEstado(): void {
    this.confirmandoCambioEstado = false;
    this.nuevoEstadoSeleccionado = null;
    this.observacionesCambio = '';
  }

  aceptarCambioEstado(): void {
    if (!this.propuesta || !this.nuevoEstadoSeleccionado) return;

    const request: CambiarEstadoRequest = {
      propuestaId: this.propuesta.id,
      estadoId: this.nuevoEstadoSeleccionado,
      observaciones: this.observacionesCambio
    };

    this.propuestaService.cambiarEstado(request).subscribe({
      next: () => {
        this.exito = 'Estado actualizado correctamente';
        this.confirmandoCambioEstado = false;
        setTimeout(() => {
          this.cargarPropuesta(this.propuesta!.id);
          this.exito = '';
        }, 1500);
      },
      error: (error) => {
        this.error = error.error?.message || 'Error al cambiar el estado';
      }
    });
  }

  editar(): void {
    if (this.propuesta) {
      this.router.navigate(['/propuestas', this.propuesta.id, 'editar']);
    }
  }

  volver(): void {
    this.router.navigate(['/propuestas']);
  }

  getEstadoBadgeClass(estadoId: number): string {
    const mapa: { [key: number]: string } = {
      1: 'badge-warning',
      2: 'badge-info',
      3: 'badge-primary',
      4: 'badge-success',
      5: 'badge-danger',
      6: 'badge-secondary'
    };
    return mapa[estadoId] || 'badge-secondary';
  }

  formatearFecha(fecha: Date | string | undefined): string {
    if (!fecha) return 'N/A';
    return new Date(fecha).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
