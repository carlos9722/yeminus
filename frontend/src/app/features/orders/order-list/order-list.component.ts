import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { OrderFilter, OrderListItem, OrderStatus } from '../../../core/models/order.models';
import { OrdersService } from '../../../core/services/orders.service';
import { stripIdentityDocDigits } from '../../../core/utils/identity-doc.formatter';
import {
  ORDER_STATUS_OPTIONS,
  orderStatusLabel
} from '../../../core/utils/order-status.util';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.scss'
})
export class OrderListComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly fb = inject(FormBuilder);

  protected readonly statusOptions = ORDER_STATUS_OPTIONS;
  protected readonly orderStatusLabel = orderStatusLabel;
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly orders = signal<OrderListItem[]>([]);

  protected readonly filterForm = this.fb.group({
    status: ['' as OrderStatus | ''],
    technicianName: [''],
    technicianSpecialty: [''],
    clientName: [''],
    clientIdentityDoc: [''],
    createdFrom: [''],
    createdTo: ['']
  });

  ngOnInit(): void {
    this.search();
  }

  protected search(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    const raw = this.filterForm.getRawValue();

    if (raw.createdFrom && raw.createdTo && raw.createdFrom > raw.createdTo) {
      this.errorMessage.set('La fecha "desde" no puede ser mayor que la fecha "hasta".');
      this.isLoading.set(false);
      return;
    }

    const docDigits = raw.clientIdentityDoc?.trim()
      ? stripIdentityDocDigits(raw.clientIdentityDoc)
      : '';

    const filter: OrderFilter = {
      status: raw.status || undefined,
      technicianName: raw.technicianName?.trim() || undefined,
      technicianSpecialty: raw.technicianSpecialty?.trim() || undefined,
      clientName: raw.clientName?.trim() || undefined,
      clientIdentityDoc: docDigits || undefined,
      createdFrom: raw.createdFrom || undefined,
      createdTo: raw.createdTo || undefined
    };

    this.ordersService.search(filter).subscribe({
      next: (data) => {
        this.orders.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar las órdenes.');
        this.isLoading.set(false);
      }
    });
  }

  protected clearFilters(): void {
    this.filterForm.reset({
      status: '',
      technicianName: '',
      technicianSpecialty: '',
      clientName: '',
      clientIdentityDoc: '',
      createdFrom: '',
      createdTo: ''
    });
    this.search();
  }

  protected deleteOrder(order: OrderListItem): void {
    const confirmed = confirm(`¿Eliminar la orden #${order.id}?`);
    if (!confirmed) {
      return;
    }

    this.ordersService.delete(order.id).subscribe({
      next: () => this.search(),
      error: (err) => {
        alert(err.error?.message ?? 'No se pudo eliminar la orden.');
      }
    });
  }

  protected formatDate(iso: string): string {
    return new Date(iso).toLocaleString('es-CO', {
      dateStyle: 'short',
      timeStyle: 'short'
    });
  }
}
