import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditListModalComponent } from './edit-list-modal.component';

describe('EditListModalComponent', () => {
  let component: EditListModalComponent;
  let fixture: ComponentFixture<EditListModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditListModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditListModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
