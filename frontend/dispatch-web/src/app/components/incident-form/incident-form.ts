import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  Incident,
  IncidentPriority,
  IncidentType,
} from '../../models/incident';
import { IncidentApi } from '../../services/incident-api';

@Component({
  selector: 'app-incident-form',
  imports: [ReactiveFormsModule],
  templateUrl: './incident-form.html',
  styleUrl: './incident-form.css',
})
export class IncidentForm {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(IncidentApi);

  // Emits the created incident so the parent can update the list
  readonly created = output<Incident>();

  readonly types: IncidentType[] = ['Fire', 'Medical', 'Police', 'Municipal'];
  readonly priorities: IncidentPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  readonly submitting = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    type: ['Fire' as IncidentType, Validators.required],
    priority: ['Medium' as IncidentPriority, Validators.required],
    address: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(1000)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.api.create(this.form.getRawValue()).subscribe({
      next: (incident) => {
        this.created.emit(incident);
        this.form.reset({ type: 'Fire', priority: 'Medium', address: '', description: '' });
        this.submitting.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not create the incident.');
        this.submitting.set(false);
      },
    });
  }
}
