import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { Technician } from '../../../core/models/technician.models';
import { TechniciansService } from '../../../core/services/technicians.service';

@Component({
  selector: 'app-technician-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './technician-list.component.html',
  styleUrl: './technician-list.component.scss'
})
export class TechnicianListComponent implements OnInit {
  private readonly techniciansService = inject(TechniciansService);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly technicians = signal<Technician[]>([]);

  ngOnInit(): void {
    this.loadTechnicians();
  }

  protected loadTechnicians(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.techniciansService.getAll().subscribe({
      next: (data) => {
        this.technicians.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la lista de técnicos.');
        this.isLoading.set(false);
      }
    });
  }

  protected deleteTechnician(technician: Technician): void {
    const confirmed = confirm(`¿Eliminar al técnico "${technician.fullName}"?`);
    if (!confirmed) {
      return;
    }

    this.techniciansService.delete(technician.id).subscribe({
      next: () => this.loadTechnicians(),
      error: (err) => {
        const message = err.error?.message ?? 'No se pudo eliminar el técnico.';
        alert(message);
      }
    });
  }
}
