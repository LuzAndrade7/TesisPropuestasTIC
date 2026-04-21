import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropuestaService } from '../../services/propuesta.service';

@Component({
  selector: 'app-asignar-estudiantes',
  templateUrl: './asignar-estudiantes.component.html',
  styleUrls: ['./asignar-estudiantes.component.css']
})
export class AsignarEstudiantesComponent implements OnInit {
  propuesta: any = null;
  modulos: any[] = [];
  isLoading: boolean = true;
  isSaving: boolean = false;

  constructor(
    private propuestaService: PropuestaService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.cargarPropuesta(params['id']);
      }
    });
  }

  cargarPropuesta(id: number): void {
    this.propuestaService.obtenerPropuesta(id).subscribe({
      next: (data) => {
        this.propuesta = data;
        this.modulos = data.modulos || [];
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error cargando propuesta:', err);
        this.isLoading = false;
        alert('Error al cargar la propuesta');
        this.router.navigate(['/dashboard']);
      }
    });
  }

  guardarAsignacion(): void {
    this.isSaving = true;
    const asignaciones = this.modulos.map(m => ({
      moduloId: m.id,
      estudianteNombre: m.estudianteAsignado || ''
    }));

    this.propuestaService.asignarEstudiantes(this.propuesta.id, asignaciones).subscribe({
      next: () => {
        this.isSaving = false;
        alert('Estudiantes asignados correctamente');
        this.router.navigate(['/detalle', this.propuesta.id]);
      },
      error: (err) => {
        this.isSaving = false;
        console.error('Error asignando estudiantes:', err);
        alert('Error al asignar estudiantes');
      }
    });
  }

  cancelar(): void {
    this.router.navigate(['/detalle', this.propuesta.id]);
  }
}
