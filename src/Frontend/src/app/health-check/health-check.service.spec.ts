import { TestBed } from '@angular/core/testing';

import { HealthCheckService } from './health-check.service';

describe('HealthCheckServiceService', () => {
  let service: HealthCheckService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HealthCheckService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
