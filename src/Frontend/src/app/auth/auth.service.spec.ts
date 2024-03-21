import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch user info', () => {
    const mockUser = {
      id: '1',
      username: 'test',
      email: 'test@test.com',
      firstName: 'Test',
      lastName: 'User',
      maxPlayersInList: 5,
      maxListsCount: 10
    };

    service.loadUser().add(() => {
      const userSignal = (service as any).userSignal
      expect(userSignal()).toEqual(mockUser);
    });

    const req = httpMock.expectOne('/user-info');
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

  it('should delete account', () => {
    service.deleteAccount().add(() => {
      const userSignal = (service as any).userSignal
      expect(userSignal()).toBeNull();
    });

    const req = httpMock.expectOne('/account');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
