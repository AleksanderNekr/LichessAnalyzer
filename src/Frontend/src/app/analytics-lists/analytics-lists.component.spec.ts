import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnalyticsListsComponent } from './analytics-lists.component';

describe('AnalyticsListsComponent', () => {
  let component: AnalyticsListsComponent;
  let fixture: ComponentFixture<AnalyticsListsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AnalyticsListsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AnalyticsListsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
