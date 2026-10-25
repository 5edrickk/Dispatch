import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';

import { Incident } from '../../models/incident';
import { IncidentApi } from '../../services/incident-api';
import { StatusBadge } from '../status-badge/status-badge';

@Component({
  selector: 'app-incident-list',
  imports: [DatePipe, StatusBadge],
  templateUrl: './incident-list.html',
  styleUrl: './incident-list.css',
})
export class IncidentList implements OnInit {
  private readonly api = inject(IncidentApi);

  readonly incidents = signal<Incident[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.list().subscribe({
      next: (list) => {
        this.incidents.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load incidents. Is the API running?');
        this.loading.set(false);
      },
    });
  }
}
