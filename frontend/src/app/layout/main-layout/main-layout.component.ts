import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IsActiveMatchOptions, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { UserProfile } from '../../core/models/auth.models';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  label: string;
  route: string;
}

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent implements OnInit {
  private readonly auth = inject(AuthService);

  protected readonly user = signal<UserProfile | null>(null);

  protected readonly navItems: NavItem[] = [
    { label: 'Inicio', route: '/dashboard' },
    { label: 'Clientes', route: '/clients' },
    { label: 'Técnicos', route: '/technicians' },
    { label: 'Órdenes', route: '/orders' }
  ];

  ngOnInit(): void {
    if (this.auth.currentUser()) {
      this.user.set(this.auth.currentUser());
      return;
    }

    this.auth.loadSession().subscribe({
      next: (profile) => this.user.set(profile),
      error: () => this.auth.logout()
    });
  }

  private readonly dashboardLinkOptions: IsActiveMatchOptions = {
    paths: 'exact',
    queryParams: 'ignored',
    matrixParams: 'ignored',
    fragment: 'ignored'
  };

  private readonly sectionLinkOptions: IsActiveMatchOptions = {
    paths: 'subset',
    queryParams: 'ignored',
    matrixParams: 'ignored',
    fragment: 'ignored'
  };

  protected linkActiveOptions(route: string): IsActiveMatchOptions {
    return route === '/dashboard' ? this.dashboardLinkOptions : this.sectionLinkOptions;
  }

  protected logout(): void {
    this.auth.logout();
  }
}
