import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Docente } from './models/propuesta.model';
import { PropuestaService } from './services/propuesta.service';

/**
 * Componente raiz de la aplicacion.
 * Contiene el encabezado institucional y el routerOutlet.
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  /**
   * Titulo institucional mostrado en el header principal.
   */
  readonly tituloInstitucional = 'FACULTAD DE INGENIERIA DE SISTEMAS - EPN';

  /**
   * Subtitulo del modulo visible bajo el titulo institucional.
   */
  readonly subtituloModulo = 'CREAR PROPUESTA - F_AA_233A';

  /**
   * Docente activo usado para pintar el nombre y las iniciales del perfil.
   */
  readonly docenteActual = signal<Docente | null>(null);

  /**
   * Nombre completo calculado desde el docente obtenido del backend.
   */
  readonly nombreDocente = computed(() => {
    const docente = this.docenteActual();

    if (!docente) {
      return 'Docente';
    }

    return `${docente.nombres} ${docente.apellidos}`.trim();
  });

  /**
   * Iniciales calculadas desde nombres y apellidos del docente.
   */
  readonly inicialesDocente = computed(() => {
    const docente = this.docenteActual();

    if (!docente) {
      return 'DC';
    }

    const inicialNombre = docente.nombres?.trim().charAt(0) ?? '';
    const inicialApellido = docente.apellidos?.trim().charAt(0) ?? '';

    return `${inicialNombre}${inicialApellido}`.toUpperCase();
  });

  /**
   * Crea el componente raiz e inyecta el servicio de propuestas.
   */
  constructor(private propuestaService: PropuestaService) {}

  /**
   * Inicializa el header cargando el docente activo.
   */
  ngOnInit(): void {
    this.cargarDocenteActual();
  }

  /**
   * Carga el primer docente disponible para mostrarlo en el header.
   */
  private cargarDocenteActual(): void {
    this.propuestaService.obtenerDocentes().subscribe({
      next: (docentes) => {
        this.docenteActual.set(docentes[0] ?? null);
      },
      error: () => {
        this.docenteActual.set(null);
      }
    });
  }
}
