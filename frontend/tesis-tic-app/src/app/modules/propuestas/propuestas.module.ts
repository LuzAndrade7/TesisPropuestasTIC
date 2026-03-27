import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PropuestasRoutingModule } from './propuestas-routing.module';
import { ListaPropuestasComponent } from './components/lista-propuestas.component';
import { CrearPropuestaComponent } from './components/crear-propuesta.component';
import { DetallePropuestaComponent } from './components/detalle-propuesta.component';

@NgModule({
  declarations: [
    ListaPropuestasComponent,
    CrearPropuestaComponent,
    DetallePropuestaComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PropuestasRoutingModule
  ]
})
export class PropuestasModule { }
