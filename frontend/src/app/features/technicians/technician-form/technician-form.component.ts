import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TechnicianFormValue } from '../../../core/models/technician.models';
import { TechniciansService } from '../../../core/services/technicians.service';
import {
  buildFullPhone,
  PHONE_PREFIX,
  phoneNumberValidator
} from '../../../core/validators/client.validators';
import { extractPhoneDigits } from '../../../core/utils/phone.formatter';

@Component({
  selector: 'app-technician-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './technician-form.component.html',
  styleUrl: './technician-form.component.scss'
})
export class TechnicianFormComponent implements OnInit {
  private readonly techniciansService = inject(TechniciansService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  protected readonly phonePrefix = PHONE_PREFIX;
  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly pageTitle = signal('Nuevo técnico');
  protected readonly showValidationErrors = signal(false);

  private technicianId: number | null = null;

  protected readonly form = this.fb.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    phoneNumber: ['', [phoneNumberValidator()]],
    specialty: ['', [Validators.required, Validators.maxLength(100)]]
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    this.technicianId = Number(idParam);
    this.pageTitle.set('Editar técnico');
    this.loadTechnician(this.technicianId);
  }

  protected isInvalid(field: 'fullName' | 'phoneNumber' | 'specialty'): boolean {
    const control = this.form.get(field);
    if (!control) {
      return false;
    }

    return control.invalid && (control.touched || this.showValidationErrors());
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
      this.errorMessage.set('Completa los campos obligatorios.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    const raw = this.form.getRawValue();
    const payload: TechnicianFormValue = {
      fullName: (raw.fullName ?? '').trim(),
      phone: buildFullPhone(raw.phoneNumber ?? ''),
      specialty: (raw.specialty ?? '').trim()
    };

    const request$ = this.technicianId
      ? this.techniciansService.update(this.technicianId, payload)
      : this.techniciansService.create(payload);

    request$.subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.router.navigate(['/technicians']);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(err.error?.message ?? 'No se pudo guardar el técnico.');
      }
    });
  }

  private loadTechnician(id: number): void {
    this.isLoading.set(true);

    this.techniciansService.getById(id).subscribe({
      next: (tech) => {
        this.form.patchValue({
          fullName: tech.fullName,
          phoneNumber: extractPhoneDigits(tech.phone),
          specialty: tech.specialty
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Técnico no encontrado.');
      }
    });
  }
}
