import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'TesisTIC - Gestión de Propuestas';
  isLoginPage: boolean = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Redirigir al dashboard automáticamente (sin login)
    if (this.router.url === '/' || this.router.url === '') {
      this.router.navigate(['/dashboard']);
    }
    
    this.router.events.subscribe(() => {
      this.isLoginPage = this.router.url === '/login';
    });
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}

