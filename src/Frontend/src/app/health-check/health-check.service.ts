import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { BehaviorSubject, catchError, map, Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class HealthCheckService {
  private status: BehaviorSubject<string> = new BehaviorSubject('unknown');
  public readonly status$: Observable<string> = this.status.asObservable();

  constructor(private http: HttpClient) {
  }

  checkHealth(): void {
    this.http.get('_health')
      .pipe(
        map(() => this.status.next('good')),
        catchError((error: HttpErrorResponse) => {
          this.status.next('bad');
          throw error;
        })
      )
      .subscribe();
  }
}
