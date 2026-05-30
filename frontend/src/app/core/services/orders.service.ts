import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  OrderDetail,
  OrderFilter,
  OrderFormValue,
  OrderListItem,
  OrderStatus
} from '../models/order.models';

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/orders';

  search(filter: OrderFilter): Observable<OrderListItem[]> {
    let params = new HttpParams();

    if (filter.status) {
      params = params.set('status', filter.status);
    }
    if (filter.technicianName?.trim()) {
      params = params.set('technicianName', filter.technicianName.trim());
    }
    if (filter.technicianSpecialty?.trim()) {
      params = params.set('technicianSpecialty', filter.technicianSpecialty.trim());
    }
    if (filter.clientName?.trim()) {
      params = params.set('clientName', filter.clientName.trim());
    }
    if (filter.clientIdentityDoc?.trim()) {
      params = params.set('clientIdentityDoc', filter.clientIdentityDoc.trim());
    }
    if (filter.createdFrom) {
      params = params.set('createdFrom', filter.createdFrom);
    }
    if (filter.createdTo) {
      params = params.set('createdTo', filter.createdTo);
    }

    return this.http.get<OrderListItem[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<OrderDetail> {
    return this.http.get<OrderDetail>(`${this.baseUrl}/${id}`);
  }

  create(payload: Omit<OrderFormValue, 'status'> & { status?: OrderStatus }): Observable<OrderDetail> {
    return this.http.post<OrderDetail>(this.baseUrl, payload);
  }

  update(id: number, payload: OrderFormValue): Observable<OrderDetail> {
    return this.http.put<OrderDetail>(`${this.baseUrl}/${id}`, payload);
  }

  changeStatus(id: number, status: OrderStatus): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, { status });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
