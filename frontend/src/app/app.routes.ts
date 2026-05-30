import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { ClientFormComponent } from './features/clients/client-form/client-form.component';
import { ClientListComponent } from './features/clients/client-list/client-list.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { OrderFormComponent } from './features/orders/order-form/order-form.component';
import { OrderListComponent } from './features/orders/order-list/order-list.component';
import { TechnicianFormComponent } from './features/technicians/technician-form/technician-form.component';
import { TechnicianListComponent } from './features/technicians/technician-list/technician-list.component';
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
      { path: 'technicians', component: TechnicianListComponent },
      { path: 'technicians/new', component: TechnicianFormComponent },
      { path: 'technicians/:id/edit', component: TechnicianFormComponent },
      { path: 'orders', component: OrderListComponent },
      { path: 'orders/new', component: OrderFormComponent },
      { path: 'orders/:id/edit', component: OrderFormComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
