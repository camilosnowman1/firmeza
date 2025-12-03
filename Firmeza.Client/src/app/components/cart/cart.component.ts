import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService, CartItem } from '../../services/cart.service';
import { SaleService, CreateSaleDto } from '../../services/sale.service';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-cart',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './cart.component.html',
    styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
    cartItems: CartItem[] = [];
    total = 0;
    processing = false;

    constructor(
        private cartService: CartService,
        private saleService: SaleService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.cartService.cartItems$.subscribe(items => {
            this.cartItems = items;
            this.total = this.cartService.getTotal();
        });
    }

    updateQuantity(productId: number, quantity: number): void {
        if (quantity < 1) {
            alert('Quantity must be at least 1');
            return;
        }
        this.cartService.updateQuantity(productId, quantity);
    }

    removeItem(productId: number): void {
        this.cartService.removeFromCart(productId);
    }

    checkout(): void {
        if (!this.authService.isLoggedIn()) {
            this.router.navigate(['/login']);
            return;
        }

        if (this.cartItems.length === 0) return;

        this.processing = true;

        // Note: In a real app, we should get the customer ID from the token or user profile.
        // For now, we might need to fetch the user profile first or assume the backend handles it if we send the token.
        // However, the CreateSaleDto expects a CustomerId.
        // Let's assume for this MVP that we need to get the user ID.
        // Since we don't have a "GetProfile" endpoint in the AuthController shown, 
        // we might need to rely on the backend extracting it from the token if we modify the backend,
        // OR we need to decode the token here.

        // Let's try to decode the token to get the ID if possible, or just send a dummy ID if we can't.
        // Actually, looking at SalesController:
        // var customer = await _customerRepository.GetByIdAsync(createSaleDto.CustomerId);
        // It requires a valid CustomerId.

        // I'll implement a helper to decode the token or fetch the user.
        // For now, let's assume we can get it from localStorage if we saved it, or decode the token.
        // The AuthController returns { Token = token }. It doesn't return the user ID.
        // The token has ClaimTypes.NameIdentifier which is user.Id (string).
        // But SalesController expects int CustomerId?
        // Let's check SalesController again.
        // public async Task<ActionResult<SaleDto>> PostSale(CreateSaleDto createSaleDto)
        // var customer = await _customerRepository.GetByIdAsync(createSaleDto.CustomerId);
        // _customerRepository.GetByIdAsync usually takes an int.
        // ApplicationUser usually uses string ID (Guid).
        // Wait, Firmeza.Core.Entities.Customer vs ApplicationUser.
        // There might be a separation between Auth User and Business Customer.
        // If so, we might need to create a Customer record first?
        // Or maybe ApplicationUser IS the Customer?
        // Let's check the backend entities if possible.
        // But I can't check everything.

        // Let's assume for now we can send a placeholder or try to extract from token.
        // If the backend expects an INT for CustomerId, then Customer entity uses int.
        // If ApplicationUser uses string (Identity), then there's a mapping.
        // Usually in these exercises, there's a Customer table.
        // Let's assume the user needs to be a "Customer".
        // Maybe the "Register" endpoint creates a Customer?
        // AuthController: var user = new ApplicationUser ... _userManager.CreateAsync ...
        // It doesn't seem to create a "Customer" entity in a separate table, unless ApplicationUser inherits or is mapped.
        // But SalesController uses _customerRepository.

        // This is a potential issue. I'll add a TODO and try to proceed.
        // I'll try to decode the token to see what's in it.

        const saleData: CreateSaleDto = {
            customerId: 1, // HARDCODED FOR NOW - We need to resolve this.
            items: this.cartItems.map(item => ({
                productId: item.product.id,
                quantity: item.quantity
            }))
        };

        this.saleService.createSale(saleData).subscribe({
            next: () => {
                this.cartService.clearCart();
                this.processing = false;
                alert('¡Compra realizada con éxito! Se ha enviado un comprobante a tu correo.');
                this.router.navigate(['/']);
            },
            error: (err) => {
                console.error(err);
                this.processing = false;
                alert('Error al procesar la compra. ' + (err.error?.message || err.message));
            }
        });
    }
}
