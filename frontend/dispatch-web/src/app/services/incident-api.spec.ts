import { TestBed } from '@angular/core/testing';

import { IncidentApi } from './incident-api';

describe('IncidentApi', () => {
  let service: IncidentApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IncidentApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
