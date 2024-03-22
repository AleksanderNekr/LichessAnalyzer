import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardAreaComponent } from './dashboard-area.component';

describe('DashboardAreaComponent', () => {
  let component: DashboardAreaComponent;
  let fixture: ComponentFixture<DashboardAreaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardAreaComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DashboardAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
