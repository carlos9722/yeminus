import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthService } from '../../core/services/auth.service';
import { UserProfile } from '../../core/models/auth.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly auth = inject(AuthService);

  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly user = signal<UserProfile | null>(null);

  ngOnInit(): void {
    if (this.auth.currentUser()) {
      this.user.set(this.auth.currentUser());
      this.isLoading.set(false);
      return;
    }

    this.auth.loadSession().subscribe({
      next: (profile) => {
        this.user.set(profile);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la sesión.');
        this.isLoading.set(false);
      }
    });
  }
}
