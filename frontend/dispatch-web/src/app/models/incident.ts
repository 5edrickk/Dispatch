export type IncidentType = 'Fire' | 'Medical' | 'Police' | 'Municipal';
export type IncidentPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type IncidentStatus = 'New' | 'Dispatched' | 'InProgress' | 'Closed';

export interface Incident {
  id: number;
  type: IncidentType;
  priority: IncidentPriority;
  status: IncidentStatus;
  address: string;
  description: string;
  createdAt: string; // ISO 8601 (UTC)
  updatedAt: string;
  closedAt: string | null;
}

// UX only: the API remains the source of truth for valid transitions.
const NEXT_STATUS: Record<IncidentStatus, IncidentStatus | null> = {
  New: 'Dispatched',
  Dispatched: 'InProgress',
  InProgress: 'Closed',
  Closed: null,
};

const NEXT_STATUS_LABEL: Record<IncidentStatus, string | null> = {
  New: 'Dispatch',
  Dispatched: 'Start',
  InProgress: 'Close',
  Closed: null,
};

const STATUS_LABEL: Record<IncidentStatus, string> = {
  New: 'New',
  Dispatched: 'Dispatched',
  InProgress: 'In progress',
  Closed: 'Closed',
};

const PRIORITY_WEIGHT: Record<IncidentPriority, number> = {
  Low: 1,
  Medium: 2,
  High: 3,
  Critical: 4,
};

export function nextStatus(status: IncidentStatus): IncidentStatus | null {
  return NEXT_STATUS[status];
}

export function nextStatusLabel(status: IncidentStatus): string | null {
  return NEXT_STATUS_LABEL[status];
}

export function statusLabel(status: IncidentStatus): string {
  return STATUS_LABEL[status];
}

export function priorityWeight(priority: IncidentPriority): number {
  return PRIORITY_WEIGHT[priority];
}
