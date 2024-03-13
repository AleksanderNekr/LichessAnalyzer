import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPlayersModalComponent } from './add-players-modal.component';

describe('CreateListModalComponent', () => {
  let component: AddPlayersModalComponent;
  let fixture: ComponentFixture<AddPlayersModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPlayersModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddPlayersModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
