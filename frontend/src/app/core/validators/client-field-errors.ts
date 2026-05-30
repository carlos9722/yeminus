export type ClientFormField = 'fullName' | 'identityDoc' | 'address' | 'phoneNumber';

export const CLIENT_FORM_FIELDS: ClientFormField[] = [
  'fullName',
  'identityDoc',
  'address',
  'phoneNumber'
];

export const CLIENT_FIELD_LABELS: Record<ClientFormField, string> = {
  fullName: 'Nombre completo',
  identityDoc: 'Número de documento',
  address: 'Dirección',
  phoneNumber: 'Teléfono'
};

const ERROR_MESSAGES: Record<ClientFormField, Record<string, string>> = {
  fullName: {
    required: 'El nombre completo es obligatorio.',
    maxlength: 'Máximo 150 caracteres.'
  },
  identityDoc: {
    required: 'El número de documento es obligatorio.',
    identityDocMin: 'El documento debe tener al menos 6 dígitos.',
    identityDocMax: 'El documento no puede superar 15 dígitos.'
  },
  address: {
    required: 'La dirección es obligatoria.',
    maxlength: 'Máximo 250 caracteres.'
  },
  phoneNumber: {
    required: 'Ingresa los 10 dígitos del teléfono.',
    phoneIncomplete: 'Faltan dígitos: deben ser 10 después de +57.',
    phoneTooLong: 'Solo se permiten 10 dígitos después de +57.'
  }
};

export function resolveClientFieldError(
  field: ClientFormField,
  errors: Record<string, unknown> | null
): string | null {
  if (!errors) {
    return null;
  }

  const messages = ERROR_MESSAGES[field];
  const key = Object.keys(errors).find((k) => messages[k]);

  return key ? messages[key] : 'Valor inválido.';
}
