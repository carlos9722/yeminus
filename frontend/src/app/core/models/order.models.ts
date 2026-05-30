export type OrderStatus = 'Pendiente' | 'EnProgreso' | 'Finalizada';

export interface OrderListItem {
  id: number;
  createdAt: string;
  status: OrderStatus;
  description: string;
  technicianName: string;
  technicianSpecialty: string;
  clientName: string;
  clientIdentityDoc: string;
}

export interface OrderDetail {
  id: number;
  createdAt: string;
  status: OrderStatus;
  description: string;
  technicianId: number;
  technicianName: string;
  technicianSpecialty: string;
  clientId: number;
  clientName: string;
  clientIdentityDoc: string;
  updatedAt: string;
}

export interface OrderFormValue {
  description: string;
  technicianId: number;
  clientId: number;
  status: OrderStatus;
}

export interface OrderFilter {
  status?: OrderStatus | '';
  technicianName?: string;
  technicianSpecialty?: string;
  clientName?: string;
  clientIdentityDoc?: string;
  createdFrom?: string;
  createdTo?: string;
}
