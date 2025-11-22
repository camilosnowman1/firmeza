import { TestBed } from '@angular/core/testing';
import { CartService } from './cart.service';
import { Product } from './product.service';

describe('CartService', () => {
    let service: CartService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(CartService);
        service.clearCart(); // Ensure clean state
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should add items to cart', () => {
        const product: Product = { id: 1, name: 'Test Product', description: 'Desc', price: 100, stock: 10 };
        service.addToCart(product);

        expect(service.getItemCount()).toBe(1);
        expect(service.getTotal()).toBe(100);
    });

    it('should calculate total correctly with multiple items', () => {
        const product1: Product = { id: 1, name: 'P1', description: 'D1', price: 10, stock: 10 };
        const product2: Product = { id: 2, name: 'P2', description: 'D2', price: 20, stock: 10 };

        service.addToCart(product1, 2); // 2 * 10 = 20
        service.addToCart(product2, 1); // 1 * 20 = 20

        expect(service.getItemCount()).toBe(3);
        expect(service.getTotal()).toBe(40);
    });

    it('should remove items from cart', () => {
        const product: Product = { id: 1, name: 'Test Product', description: 'Desc', price: 100, stock: 10 };
        service.addToCart(product);
        service.removeFromCart(product.id);

        expect(service.getItemCount()).toBe(0);
    });
});
