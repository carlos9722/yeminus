import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { Technician, TechnicianFormValue } from '../models/technician.models';

@Injectable({ providedIn: 'root' })
export class TechniciansService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/technicians';

  getAll(): Observable<Technician[]> {
    return this.http.get<Technician[]>(this.baseUrl);
  }

  getById(id: number): Observable<Technician> {
    return this.http.get<Technician>(`${this.baseUrl}/${id}`);
  }

  create(payload: TechnicianFormValue): Observable<Technician> {
    return this.http.post<Technician>(this.baseUrl, payload);
  }

  update(id: number, payload: TechnicianFormValue): Observable<Technician> {
    return this.http.put<Technician>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
