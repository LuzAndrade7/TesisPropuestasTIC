import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  email: string = 'ana.lopez@epn.edu.ec';
  password: string = '••••••••';
  isLoading: boolean = false;

  constructor(private router: Router) {}

  ngOnInit(): void {}

  onLogin(): void {
    this.isLoading = true;
    // Simular login
    setTimeout(() => {
      this.isLoading = false;
      this.router.navigate(['/dashboard']);
    }, 500);
  }
}
