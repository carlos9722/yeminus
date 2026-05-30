import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { Client } from '../../../core/models/client.models';
import { OrderStatus } from '../../../core/models/order.models';
import { Technician } from '../../../core/models/technician.models';
import { ClientsService } from '../../../core/services/clients.service';
import { OrdersService } from '../../../core/services/orders.service';
import { TechniciansService } from '../../../core/services/technicians.service';
import { ORDER_STATUS_OPTIONS } from '../../../core/utils/order-status.util';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './order-form.component.html',
  styleUrl: './order-form.component.scss'
})
export class OrderFormComponent implements OnInit {
  private readonly ordersService = inject(OrdersService);
  private readonly clientsService = inject(ClientsService);
  private readonly techniciansService = inject(TechniciansService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  protected readonly statusOptions = ORDER_STATUS_OPTIONS;
  protected readonly isEditMode = signal(false);
  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly pageTitle = signal('Nueva orden');
  protected readonly clients = signal<Client[]>([]);
  protected readonly technicians = signal<Technician[]>([]);

  private orderId: number | null = null;

  protected readonly form = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(5)]],
    technicianId: [0, [Validators.required, Validators.min(1)]],
    clientId: [0, [Validators.required, Validators.min(1)]],
    status: ['Pendiente' as OrderStatus, Validators.required]
  });

  ngOnInit(): void {
    this.loadLookups();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    this.orderId = Number(idParam);
    this.isEditMode.set(true);
    this.pageTitle.set('Editar orden');
    this.loadOrder(this.orderId);
  }

  protected onSubmit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      this.errorMessage.set('Completa los campos obligatorios.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();

    if (this.orderId) {
      this.ordersService
        .update(this.orderId, {
          description: (raw.description ?? '').trim(),
          technicianId: Number(raw.technicianId),
          clientId: Number(raw.clientId),
          status: raw.status as OrderStatus
        })
        .subscribe({
          next: () => {
            this.isSubmitting.set(false);
            this.router.navigate(['/orders']);
          },
          error: (err) => this.handleError(err)
        });
    } else {
      this.ordersService
        .create({
          description: (raw.description ?? '').trim(),
          technicianId: Number(raw.technicianId),
          clientId: Number(raw.clientId)
        })
        .subscribe({
          next: () => {
            this.isSubmitting.set(false);
            this.router.navigate(['/orders']);
          },
          error: (err) => this.handleError(err)
        });
    }
  }

  private loadLookups(): void {
    this.clientsService.getAll().subscribe({
      next: (data) => this.clients.set(data),
      error: () => this.errorMessage.set('No se pudieron cargar los clientes.')
    });

    this.techniciansService.getAll().subscribe({
      next: (data) => this.technicians.set(data),
      error: () => this.errorMessage.set('No se pudieron cargar los técnicos.')
    });
  }

  private loadOrder(id: number): void {
    this.isLoading.set(true);

    this.ordersService.getById(id).subscribe({
      next: (order) => {
        this.form.patchValue({
          description: order.description,
          technicianId: order.technicianId,
          clientId: order.clientId,
          status: order.status
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Orden no encontrada.');
      }
    });
  }

  private handleError(err: { error?: { message?: string } }): void {
    this.isSubmitting.set(false);
    this.errorMessage.set(err.error?.message ?? 'No se pudo guardar la orden.');
  }
}
