export interface Technician {
  id: number;
  fullName: string;
  phone: string;
  specialty: string;
  createdAt: string;
  updatedAt: string;
}

export interface TechnicianFormValue {
  fullName: string;
  phone: string;
  specialty: string;
}
