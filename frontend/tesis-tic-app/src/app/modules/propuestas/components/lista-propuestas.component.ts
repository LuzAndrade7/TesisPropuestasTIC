import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PropuestaService } from '../services/propuesta.service';
import { Propuesta, Estado } from '../../shared/models/propuesta.model';

@Component({
  selector: 'app-lista-propuestas',
  templateUrl: './lista-propuestas.component.html',
  styleUrls: ['./lista-propuestas.component.css']
})
export class ListaPropuestasComponent implements OnInit {
  propuestas: Propuesta[] = [];
  propuestasFiltradas: Propuesta[] = [];
  estados: Estado[] = [];
  estadoSeleccionado: number = 0;
  cargando: boolean = false;
  error: string = '';

  constructor(
    private propuestaService: PropuestaService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarEstados();
    this.cargarPropuestas();
  }

  cargarEstados(): void {
    this.propuestaService.obtenerEstados().subscribe({
      next: (estados) => {
        this.estados = estados;
      },
      error: (error) => {
        console.error('Error al cargar estados:', error);
        this.error = 'Error cargando los estados';
      }
    });
  }

  cargarPropuestas(): void {
    this.cargando = true;
    this.error = '';

    this.propuestaService.obtenerTodasPropuestas().subscribe({
      next: (propuestas) => {
        this.propuestas = propuestas;
        this.aplicarFiltro();
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error al cargar propuestas:', error);
        this.error = 'Error cargando las propuestas';
        this.cargando = false;
      }
    });
  }

  aplicarFiltro(): void {
    if (this.estadoSeleccionado === 0) {
      this.propuestasFiltradas = this.propuestas;
    } else {
      this.propuestasFiltradas = this.propuestas.filter(
        p => p.estadoId === this.estadoSeleccionado
      );
    }
  }

  onEstadoSeleccionado(estadoId: number): void {
    this.estadoSeleccionado = estadoId;
    this.aplicarFiltro();
  }

  irACrear(): void {
    this.router.navigate(['/propuestas/nueva']);
  }

  verDetalle(id: number): void {
    this.router.navigate(['/propuestas', id]);
  }

  editar(id: number): void {
    this.router.navigate(['/propuestas', id, 'editar']);
  }

  eliminar(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar esta propuesta?')) {
      this.propuestaService.eliminarPropuesta(id).subscribe({
        next: () => {
          this.cargarPropuestas();
        },
        error: (error) => {
          console.error('Error al eliminar propuesta:', error);
          this.error = 'Error al eliminar la propuesta';
        }
      });
    }
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
}
