import { OrderStatus } from '../models/order.models';

export const ORDER_STATUS_OPTIONS: { value: OrderStatus; label: string }[] = [
  { value: 'Pendiente', label: 'Pendiente' },
  { value: 'EnProgreso', label: 'En progreso' },
  { value: 'Finalizada', label: 'Finalizada' }
];

export function orderStatusLabel(status: OrderStatus): string {
  return ORDER_STATUS_OPTIONS.find((o) => o.value === status)?.label ?? status;
}
