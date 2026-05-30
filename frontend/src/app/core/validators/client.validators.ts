import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const PHONE_PREFIX = '+57';

export function identityDocValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = (control.value as string) ?? '';
    const digits = value.replace(/\D/g, '');

    if (!value.trim()) {
      return { required: true };
    }

    if (digits.length < 6) {
      return { identityDocMin: true };
    }

    if (digits.length > 15) {
      return { identityDocMax: true };
    }

    return null;
  };
}

export function phoneNumberValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const digits = ((control.value as string) ?? '').replace(/\D/g, '');

    if (digits.length === 0) {
      return { required: true };
    }

    if (digits.length < 10) {
      return { phoneIncomplete: true };
    }

    if (digits.length > 10) {
      return { phoneTooLong: true };
    }

    return null;
  };
}

export function buildFullPhone(phoneNumber: string): string {
  const digits = phoneNumber.replace(/\D/g, '');
  return `${PHONE_PREFIX} ${digits}`;
}
