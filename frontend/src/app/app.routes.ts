import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { ClientFormComponent } from './features/clients/client-form/client-form.component';
import { ClientListComponent } from './features/clients/client-list/client-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { OrderShellComponent } from './features/orders/order-shell/order-shell.component';
import { TechnicianShellComponent } from './features/technicians/technician-shell/technician-shell.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'clients', component: ClientListComponent },
      { path: 'clients/new', component: ClientFormComponent },
      { path: 'clients/:id/edit', component: ClientFormComponent },
      { path: 'technicians', component: TechnicianShellComponent },
      { path: 'orders', component: OrderShellComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
