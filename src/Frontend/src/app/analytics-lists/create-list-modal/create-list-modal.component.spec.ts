import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateListModalComponent } from './create-list-modal.component';

describe('CreateListModalComponent', () => {
  let component: CreateListModalComponent;
  let fixture: ComponentFixture<CreateListModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateListModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateListModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
