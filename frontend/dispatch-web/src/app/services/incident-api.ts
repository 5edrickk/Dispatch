import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_URL } from '../config';
import {
  Incident,
  IncidentPriority,
  IncidentStatus,
  IncidentType,
} from '../models/incident';

export interface IncidentFilter {
  status?: IncidentStatus;
  priority?: IncidentPriority;
  type?: IncidentType;
  includeClosed?: boolean;
}

export interface CreateIncidentRequest {
  type: IncidentType;
  priority: IncidentPriority;
  address: string;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class IncidentApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/api/incidents`;

  list(filter: IncidentFilter = {}): Observable<Incident[]> {
    let params = new HttpParams();
    if (filter.status) params = params.set('status', filter.status);
    if (filter.priority) params = params.set('priority', filter.priority);
    if (filter.type) params = params.set('type', filter.type);
    if (filter.includeClosed) params = params.set('includeClosed', true);

    return this.http.get<Incident[]>(this.baseUrl, { params });
  }

  get(id: number): Observable<Incident> {
    return this.http.get<Incident>(`${this.baseUrl}/${id}`);
  }

  create(body: CreateIncidentRequest): Observable<Incident> {
    return this.http.post<Incident>(this.baseUrl, body);
  }

  updateStatus(id: number, status: IncidentStatus): Observable<Incident> {
    return this.http.patch<Incident>(`${this.baseUrl}/${id}/status`, { status });
  }
}
