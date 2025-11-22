import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product, ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
    selector: 'app-product-list',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './product-list.component.html',
    styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
    products: Product[] = [];
    loading = true;
    error = '';

    // Pagination
    currentPage = 1;
    pageSize = 9;
    totalPages = 0;
    totalCount = 0;

    constructor(
        private productService: ProductService,
        private cartService: CartService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit(): void {
        this.loadProducts();
    }

    loadProducts(): void {
        console.log('ProductListComponent: Iniciando carga de productos...');
        this.loading = true;
        this.productService.getProducts(this.currentPage, this.pageSize).subscribe({
            next: (response) => {
                console.log('ProductListComponent: Productos recibidos:', response);
                this.products = response.data;
                this.totalCount = response.totalCount;
                this.totalPages = response.totalPages;
                this.currentPage = response.pageNumber;
                this.loading = false;
                this.cdr.detectChanges();
                console.log('ProductListComponent: Estado actualizado, loading:', this.loading, 'products:', this.products.length);
            },
            error: (err) => {
                console.error('ProductListComponent: Error al cargar productos:', err);
                this.error = 'Error al cargar productos. Asegúrate de que la API esté corriendo.';
                this.loading = false;
                this.cdr.detectChanges();
            }
        });
    }

    addToCart(product: Product): void {
        this.cartService.addToCart(product);
        alert('Producto agregado al carrito');
    }

    // Pagination methods
    goToPage(page: number): void {
        if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
            this.currentPage = page;
            this.loadProducts();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    nextPage(): void {
        this.goToPage(this.currentPage + 1);
    }

    previousPage(): void {
        this.goToPage(this.currentPage - 1);
    }

    get pageNumbers(): number[] {
        const pages: number[] = [];
        const maxPagesToShow = 5;
        let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
        let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

        if (endPage - startPage < maxPagesToShow - 1) {
            startPage = Math.max(1, endPage - maxPagesToShow + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            pages.push(i);
        }

        return pages;
    }
}
