import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Product } from './product.service';

export interface CartItem {
    product: Product;
    quantity: number;
}

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private cartItems = new BehaviorSubject<CartItem[]>([]);
    cartItems$ = this.cartItems.asObservable();

    constructor() {
        // Load from local storage if available
        const savedCart = localStorage.getItem('cart');
        if (savedCart) {
            this.cartItems.next(JSON.parse(savedCart));
        }
    }

    addToCart(product: Product, quantity: number = 1) {
        const currentItems = this.cartItems.value;
        const existingItem = currentItems.find(item => item.product.id === product.id);

        if (existingItem) {
            existingItem.quantity += quantity;
            this.cartItems.next([...currentItems]);
        } else {
            this.cartItems.next([...currentItems, { product, quantity }]);
        }
        this.saveCart();
    }

    removeFromCart(productId: number) {
        const currentItems = this.cartItems.value.filter(item => item.product.id !== productId);
        this.cartItems.next(currentItems);
        this.saveCart();
    }

    updateQuantity(productId: number, quantity: number) {
        const currentItems = this.cartItems.value;
        const item = currentItems.find(i => i.product.id === productId);
        if (item) {
            item.quantity = quantity;
            if (item.quantity <= 0) {
                this.removeFromCart(productId);
                return;
            }
            this.cartItems.next([...currentItems]);
            this.saveCart();
        }
    }

    clearCart() {
        this.cartItems.next([]);
        this.saveCart();
    }

    getTotal(): number {
        return this.cartItems.value.reduce((acc, item) => acc + (item.product.price * item.quantity), 0);
    }

    getItemCount(): number {
        return this.cartItems.value.reduce((acc, item) => acc + item.quantity, 0);
    }

    private saveCart() {
        localStorage.setItem('cart', JSON.stringify(this.cartItems.value));
    }
}
