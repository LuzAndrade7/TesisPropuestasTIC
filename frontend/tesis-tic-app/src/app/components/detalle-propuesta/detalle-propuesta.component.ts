import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PropuestaService } from '../../services/propuesta.service';

@Component({
  selector: 'app-detalle-propuesta',
  templateUrl: './detalle-propuesta.component.html',
  styleUrls: ['./detalle-propuesta.component.css']
})
export class DetallePropuestaComponent implements OnInit {
  propuesta: any = null;
  isLoading: boolean = true;

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

  imprimirPropuesta(): void {
    window.print();
  }

  editarPropuesta(): void {
    this.router.navigate(['/editar', this.propuesta.id]);
  }

  asignarEstudiantes(): void {
    this.router.navigate(['/asignar', this.propuesta.id]);
  }

  volver(): void {
    this.router.navigate(['/dashboard']);
  }
}
