import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'propuestas',
    loadChildren: () => import('./modules/propuestas/propuestas.module').then(m => m.PropuestasModule)
  },
  {
    path: '',
    redirectTo: '/propuestas',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    enableTracing: false,
    useHash: false
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
