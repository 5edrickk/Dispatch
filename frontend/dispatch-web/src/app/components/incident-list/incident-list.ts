import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';

import {
  Incident,
  IncidentPriority,
  IncidentType,
  nextStatus,
  nextStatusLabel,
  priorityWeight,
} from '../../models/incident';
import { IncidentApi } from '../../services/incident-api';
import { PriorityMeter } from '../priority-meter/priority-meter';
import { StatusBadge } from '../status-badge/status-badge';

@Component({
  selector: 'app-incident-list',
  imports: [DatePipe, PriorityMeter, StatusBadge],
  templateUrl: './incident-list.html',
  styleUrl: './incident-list.css',
})
export class IncidentList implements OnInit {
  private readonly api = inject(IncidentApi);
  private errorTimer: ReturnType<typeof setTimeout> | null = null;

  readonly incidents = signal<Incident[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly actionError = signal<string | null>(null);
  readonly busyId = signal<number | null>(null);

  readonly types: IncidentType[] = ['Fire', 'Medical', 'Police', 'Municipal'];
  readonly priorities: IncidentPriority[] = ['Low', 'Medium', 'High', 'Critical'];

  readonly typeFilter = signal<IncidentType | ''>('');
  readonly priorityFilter = signal<IncidentPriority | ''>('');
  readonly showClosed = signal(false);

  // Exposed so the template can call it
  readonly nextLabel = nextStatusLabel;

  ngOnInit(): void {
  this.load();
}

load(): void {
  this.loading.set(true);
  this.error.set(null);

  this.api
    .list({
      type: this.typeFilter() || undefined,
      priority: this.priorityFilter() || undefined,
      includeClosed: this.showClosed(),
    })
    .subscribe({
      next: (list) => {
        this.incidents.set(this.sorted(list));
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load incidents. Is the API running?');
        this.loading.set(false);
      },
    });
}

onTypeChange(value: string): void {
  this.typeFilter.set(value as IncidentType | '');
  this.load();
}

onPriorityChange(value: string): void {
  this.priorityFilter.set(value as IncidentPriority | '');
  this.load();
}

onShowClosedChange(checked: boolean): void {
  this.showClosed.set(checked);
  this.load();
}

  add(incident: Incident): void {
    this.incidents.update((list) => this.sorted([...list, incident]));
  }

  advance(incident: Incident): void {
    const next = nextStatus(incident.status);
    if (!next) return;

    this.busyId.set(incident.id);
    this.clearError();

    this.api.updateStatus(incident.id, next).subscribe({
      next: (updated) => {
        this.incidents.update((list) =>
          this.sorted(list.map((i) => (i.id === updated.id ? updated : i))),
        );
        this.busyId.set(null);
      },
      error: (err) => {
        if (err.status === 0) {
          this.showError('Network error: cannot reach the API.');
        } else if (err.status === 404) {
          this.showError('This incident no longer exists.');
        } else if (err.status === 409) {
          this.showError('Invalid status transition. Please refresh the list.');
        } else {
          this.showError('Could not update the incident.');
        }
        this.busyId.set(null);
      },
    });
  }

  private showError(message: string): void {
    this.actionError.set(message);
    if (this.errorTimer) clearTimeout(this.errorTimer);
    this.errorTimer = setTimeout(() => this.actionError.set(null), 5000);
  }

  private clearError(): void {
    if (this.errorTimer) clearTimeout(this.errorTimer);
    this.errorTimer = null;
    this.actionError.set(null);
  }

  private sorted(list: Incident[]): Incident[] {
    return [...list].sort(
      (a, b) =>
        priorityWeight(b.priority) - priorityWeight(a.priority) ||
        a.createdAt.localeCompare(b.createdAt),
    );
  }
}
