import { Component, computed, input } from '@angular/core';

import { IncidentPriority, priorityWeight } from '../../models/incident';

@Component({
  selector: 'app-priority-meter',
  imports: [],
  templateUrl: './priority-meter.html',
  styleUrl: './priority-meter.css',
})
export class PriorityMeter {
  readonly priority = input.required<IncidentPriority>();

  // Bars are filled up to the priority weight (Low = 1 ... Critical = 4)
  readonly level = computed(() => priorityWeight(this.priority()));
  readonly bars = [1, 2, 3, 4];
}
