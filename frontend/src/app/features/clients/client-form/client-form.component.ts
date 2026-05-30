import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { ClientFormValue } from '../../../core/models/client.models';
import { ClientsService } from '../../../core/services/clients.service';
import {
  CLIENT_FIELD_LABELS,
  CLIENT_FORM_FIELDS,
  ClientFormField,
  resolveClientFieldError
} from '../../../core/validators/client-field-errors';
import {
  buildFullPhone,
  identityDocValidator,
  PHONE_PREFIX,
  phoneNumberValidator
} from '../../../core/validators/client.validators';
import { formatIdentityDoc } from '../../../core/utils/identity-doc.formatter';
import { extractPhoneDigits } from '../../../core/utils/phone.formatter';

@Component({
  selector: 'app-client-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './client-form.component.html',
  styleUrl: './client-form.component.scss'
})
export class ClientFormComponent implements OnInit {
  private readonly clientsService = inject(ClientsService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  protected readonly phonePrefix = PHONE_PREFIX;
  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly pageTitle = signal('Nuevo cliente');
  protected readonly showValidationErrors = signal(false);

  private clientId: number | null = null;

  protected readonly form = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    identityDoc: ['', [identityDocValidator()]],
    address: ['', [Validators.required, Validators.maxLength(250)]],
    phoneNumber: ['', [phoneNumberValidator()]]
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    this.clientId = Number(idParam);
    this.pageTitle.set('Editar cliente');
    this.loadClient(this.clientId);
  }

  protected isInvalid(field: ClientFormField): boolean {
    const control = this.form.get(field);
    if (!control) {
      return false;
    }

    return control.invalid && (control.touched || this.showValidationErrors());
  }

  protected getFieldError(field: ClientFormField): string | null {
    const control = this.form.get(field);
    if (!control || (!this.showValidationErrors() && !control.touched)) {
      return null;
    }

    return resolveClientFieldError(field, control.errors);
  }

  protected getValidationSummary(): { field: string; message: string }[] {
    if (!this.showValidationErrors()) {
      return [];
    }

    return CLIENT_FORM_FIELDS.filter((field) => this.form.get(field)?.invalid).map(
      (field) => ({
        field: CLIENT_FIELD_LABELS[field],
        message: this.getFieldError(field) ?? 'Valor inválido.'
      })
    );
  }

  protected onIdentityDocInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const formatted = formatIdentityDoc(input.value);
    const control = this.form.controls.identityDoc;

    control.setValue(formatted);
    control.markAsTouched();
    input.value = formatted;
  }

  protected onPhoneNumberInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const digits = input.value.replace(/\D/g, '').slice(0, 10);
    const control = this.form.controls.phoneNumber;

    control.setValue(digits);
    control.markAsTouched();
    input.value = digits;
  }

  protected onSubmit(): void {
    this.showValidationErrors.set(true);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      this.errorMessage.set('Completa los campos obligatorios indicados abajo.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const payload: ClientFormValue = {
      fullName: (raw.fullName ?? '').trim(),
      identityDoc: (raw.identityDoc ?? '').trim(),
      address: (raw.address ?? '').trim(),
      phone: buildFullPhone(raw.phoneNumber ?? '')
    };

    const request$ = this.clientId
      ? this.clientsService.update(this.clientId, payload)
      : this.clientsService.create(payload);

    request$.subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.router.navigate(['/clients']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.message ?? 'No se pudo guardar el cliente.');
      }
    });
  }

  private loadClient(id: number): void {
    this.isLoading.set(true);

    this.clientsService.getById(id).subscribe({
      next: (client) => {
        this.form.patchValue({
          fullName: client.fullName,
          identityDoc: formatIdentityDoc(client.identityDoc),
          address: client.address,
          phoneNumber: extractPhoneDigits(client.phone)
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Cliente no encontrado.');
      }
    });
  }
}
