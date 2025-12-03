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
                  <label for="fullName">Nombre Completo</label>
                  <input type="text" id="fullName" class="form-control" [(ngModel)]="user.fullName" name="fullName" required #nameModel="ngModel">
                  <div *ngIf="nameModel.invalid && (nameModel.dirty || nameModel.touched)" class="text-danger">
                    <div *ngIf="nameModel.errors?.['required']">Full Name is required.</div>
                  </div>
                </div>
                <div class="form-group mb-3">
                  <label for="document">Documento</label>
                  <input type="text" id="document" class="form-control" [(ngModel)]="user.document" name="document" required #docModel="ngModel">
                  <div *ngIf="docModel.invalid && (docModel.dirty || docModel.touched)" class="text-danger">
                    <div *ngIf="docModel.errors?.['required']">Document is required.</div>
                  </div>
                </div>
                <div class="form-group mb-3">
                  <label for="email">Email</label>
                  <input type="email" id="email" class="form-control" [(ngModel)]="user.email" name="email" required email #emailModel="ngModel">
                  <div *ngIf="emailModel.invalid && (emailModel.dirty || emailModel.touched)" class="text-danger">
                    <div *ngIf="emailModel.errors?.['required']">Email is required.</div>
                    <div *ngIf="emailModel.errors?.['email']">Invalid email format.</div>
                  </div>
                </div>
                <div class="form-group mb-3">
                  <label for="password">Contraseña</label>
                  <input type="password" id="password" class="form-control" [(ngModel)]="user.password" name="password" required minlength="6" #passwordModel="ngModel">
                  <div *ngIf="passwordModel.invalid && (passwordModel.dirty || passwordModel.touched)" class="text-danger">
                    <div *ngIf="passwordModel.errors?.['required']">Password is required.</div>
                    <div *ngIf="passwordModel.errors?.['minlength']">Password must be at least 6 characters.</div>
                  </div>
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
  user = { fullName: '', document: '', email: '', password: '' };
  error: string | null = null;
  success: string | null = null;

  constructor(private authService: AuthService, private router: Router) { }

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
