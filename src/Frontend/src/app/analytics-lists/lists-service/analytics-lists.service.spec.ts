import { TestBed } from '@angular/core/testing';

import { AnalyticsListsService } from './analytics-lists.service';

describe('AnalyticsListsService', () => {
  let service: AnalyticsListsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AnalyticsListsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
