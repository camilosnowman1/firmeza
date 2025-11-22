import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartItem } from './cart.service';

export interface SaleDetailDto {
    productId: number;
    quantity: number;
}

export interface CreateSaleDto {
    customerId: number; // We might need to extract this from the token or user profile
    items: SaleDetailDto[];
}

@Injectable({
    providedIn: 'root'
})
export class SaleService {
    private apiUrl = 'http://localhost:5000/api/v1/Sales';

    constructor(private http: HttpClient) { }

    createSale(saleData: CreateSaleDto): Observable<any> {
        return this.http.post(this.apiUrl, saleData);
    }
}
