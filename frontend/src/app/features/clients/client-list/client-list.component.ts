import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Client } from '../../../core/models/client.models';
import { ClientsService } from '../../../core/services/clients.service';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './client-list.component.html',
  styleUrl: './client-list.component.scss'
})
export class ClientListComponent implements OnInit {
  private readonly clientsService = inject(ClientsService);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly clients = signal<Client[]>([]);

  ngOnInit(): void {
    this.loadClients();
  }

  protected loadClients(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.clientsService.getAll().subscribe({
      next: (data) => {
        this.clients.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la lista de clientes.');
        this.isLoading.set(false);
      }
    });
  }

  protected deleteClient(client: Client): void {
    const confirmed = confirm(`¿Eliminar al cliente "${client.fullName}"?`);
    if (!confirmed) {
      return;
    }

    this.clientsService.delete(client.id).subscribe({
      next: () => this.loadClients(),
      error: (err) => {
        const message = err.error?.message ?? 'No se pudo eliminar el cliente.';
        alert(message);
      }
    });
  }
}
