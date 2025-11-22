import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule], // Añadir RouterModule
  template: `
    <div class="container mt-5">
      <div class="row justify-content-center">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h3>Registro de Nuevo Usuario</h3>
            </div>
            <div class="card-body">
              <form (ngSubmit)="onSubmit()">
                <div class="form-group mb-3">
                  <label for="email">Email</label>
                  <input type="email" id="email" class="form-control" [(ngModel)]="user.email" name="email" required>
                </div>
                <div class="form-group mb-3">
                  <label for="password">Contraseña</label>
                  <input type="password" id="password" class="form-control" [(ngModel)]="user.password" name="password" required>
                </div>
                <button type="submit" class="btn btn-primary w-100">Registrarse</button>
              </form>
              <div class="text-center mt-3">
                <a routerLink="/login">¿Ya tienes una cuenta? Inicia sesión</a>
              </div>
              <div *ngIf="error" class="alert alert-danger mt-3">{{ error }}</div>
              <div *ngIf="success" class="alert alert-success mt-3">{{ success }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  user = { email: '', password: '' };
  error: string | null = null;
  success: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    this.authService.register(this.user).subscribe({
      next: () => {
        this.success = '¡Registro exitoso! Ahora puedes iniciar sesión.';
        this.error = null;
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.error = 'Error en el registro. Por favor, inténtalo de nuevo.';
        this.success = null;
        console.error(err);
      }
    });
  }
}
