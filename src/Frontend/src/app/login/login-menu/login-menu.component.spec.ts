import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginMenuComponent } from './login-menu.component';

describe('LoginMenuComponent', () => {
  let component: LoginMenuComponent;
  let fixture: ComponentFixture<LoginMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginMenuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
