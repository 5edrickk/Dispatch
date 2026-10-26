import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Incident, IncidentStatus } from '../../models/incident';
import { StatusSummary } from './status-summary';

describe('StatusSummary', () => {
  let component: StatusSummary;
  let fixture: ComponentFixture<StatusSummary>;

  const incident = (id: number, status: IncidentStatus) => ({ id, status }) as Incident;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusSummary],
    }).compileComponents();

    fixture = TestBed.createComponent(StatusSummary);
    component = fixture.componentInstance;
  });

  it('counts incidents per status', async () => {
    fixture.componentRef.setInput('incidents', [
      incident(1, 'New'),
      incident(2, 'InProgress'),
      incident(3, 'InProgress'),
    ]);
    await fixture.whenStable();

    expect(component.counts()).toEqual({ New: 1, Dispatched: 0, InProgress: 2, Closed: 0 });
    expect(fixture.nativeElement.textContent).toContain('In progress');
  });
});
