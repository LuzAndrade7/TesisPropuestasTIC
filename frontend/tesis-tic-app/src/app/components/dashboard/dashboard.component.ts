import { Component, OnInit } from '@angular/core';
import { PropuestaService } from '../../services/propuesta.service';

interface EstadisticasUI {
  [key: string]: number;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  propuestas: any[] = [];
  estadisticas: EstadisticasUI = {};
  filtroActivo: string = 'Todos';
  isLoading: boolean = true;
  propuestasFiltradas: any[] = [];

  estados = ['Todos', 'Borrador', 'Pendiente', 'Observada', 'Aprobada', 'Rechazada'];

  constructor(private propuestaService: PropuestaService) {}

  ngOnInit(): void {
    this.cargarPropuestas();
    this.cargarEstadisticas();
  }

  cargarPropuestas(): void {
    this.propuestaService.obtenerPropuestas().subscribe({
      next: (data) => {
        this.propuestas = data;
        this.aplicarFiltro('Todos');
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error cargando propuestas:', err);
        this.isLoading = false;
      }
    });
  }

  cargarEstadisticas(): void {
    this.propuestaService.obtenerEstadisticas().subscribe({
      next: (data) => {
        this.estadisticas = {
          'Todos': data.todos || 0,
          'Borrador': data.borrador || 0,
          'Pendiente': data.pendiente || 0,
          'Observada': data.observada || 0,
          'Aprobada': data.aprobada || 0,
          'Rechazada': data.rechazada || 0
        };
      },
      error: (err) => {
        console.error('Error cargando estadísticas:', err);
        // Valores por defecto
        this.estadisticas = {
          'Todos': 0,
          'Borrador': 0,
          'Pendiente': 0,
          'Observada': 0,
          'Aprobada': 0,
          'Rechazada': 0
        };
      }
    });
  }

  aplicarFiltro(estado: string): void {
    this.filtroActivo = estado;
    if (estado === 'Todos') {
      this.propuestasFiltradas = this.propuestas;
    } else {
      this.propuestasFiltradas = this.propuestas.filter(p => p.estado === estado);
    }
  }

  obtenerColorEstado(estado: string): string {
    const colores: { [key: string]: string } = {
      'Borrador': '#9CA3AF',
      'Pendiente': '#77A4DC',
      'Observada': '#F3BD46',
      'Aprobada': '#10B981',
      'Rechazada': '#E31D1A'
    };
    return colores[estado] || '#0E2240';
  }

  obtenerIntensidadColor(estado: string): string {
    const intensidades: { [key: string]: string } = {
      'Borrador': '#eef3fb',
      'Pendiente': '#eef3fb',
      'Observada': '#fffbf0',
      'Aprobada': '#f0fdf4',
      'Rechazada': '#fef2f2'
    };
    return intensidades[estado] || '#eef3fb';
  }
}
