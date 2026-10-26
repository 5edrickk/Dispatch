import { Component, computed, input } from '@angular/core';

import { Incident, IncidentStatus, statusLabel } from '../../models/incident';

@Component({
  selector: 'app-status-summary',
  imports: [],
  templateUrl: './status-summary.html',
  styleUrl: './status-summary.css',
})
export class StatusSummary {
  // Counts only what the list has loaded, so filters and "Show closed" apply here too
  readonly incidents = input.required<Incident[]>();

  readonly statuses: IncidentStatus[] = ['New', 'Dispatched', 'InProgress', 'Closed'];

  // Exposed so the template can call it
  readonly label = statusLabel;

  readonly counts = computed(() => {
    const counts: Record<IncidentStatus, number> = { New: 0, Dispatched: 0, InProgress: 0, Closed: 0 };
    for (const incident of this.incidents()) counts[incident.status]++;
    return counts;
  });
}
