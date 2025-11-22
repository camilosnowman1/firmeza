import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // Añadir RouterModule
  template: `
    <div class="container mt-5">
      <div class="row justify-content-center">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h3>Iniciar Sesión</h3>
            </div>
            <div class="card-body">
              <form (ngSubmit)="onSubmit()">
                <div class="form-group mb-3">
                  <label for="email">Email</label>
                  <input type="email" id="email" class="form-control" [(ngModel)]="credentials.email" name="email" required>
                </div>
                <div class="form-group mb-3">
                  <label for="password">Contraseña</label>
                  <input type="password" id="password" class="form-control" [(ngModel)]="credentials.password" name="password" required>
                </div>
                <button type="submit" class="btn btn-primary w-100">Login</button>
              </form>
              <div class="text-center mt-3">
                <a routerLink="/register">¿No tienes una cuenta? Regístrate aquí</a>
              </div>
              <div *ngIf="error" class="alert alert-danger mt-3">{{ error }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  credentials = { email: '', password: '' };
  error: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = 'Error en el inicio de sesión. Por favor, verifica tus credenciales.';
        console.error(err);
      }
    });
  }
}
