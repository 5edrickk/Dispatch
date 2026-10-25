import { Component, input } from '@angular/core';

import { IncidentStatus } from '../../models/incident';

@Component({
  selector: 'app-status-badge',
  imports: [],
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css',
})
export class StatusBadge {
  // Required signal input: the parent must pass a status
  readonly status = input.required<IncidentStatus>();
}
