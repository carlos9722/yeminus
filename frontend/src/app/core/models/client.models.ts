export interface Client {
  id: number;
  fullName: string;
  identityDoc: string;
  address: string;
  phone: string;
  createdAt: string;
  updatedAt: string;
}

export interface ClientFormValue {
  fullName: string;
  identityDoc: string;
  address: string;
  phone: string;
}
