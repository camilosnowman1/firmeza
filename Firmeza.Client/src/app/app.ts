import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class AppComponent {
  title = 'Firmeza.Client';

  cartItemCount = 0;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cartService: CartService
  ) {
    this.cartService.cartItems$.subscribe(() => {
      this.cartItemCount = this.cartService.getItemCount();
    });
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
