import { computed, effect, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, throwError } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private readonly httpClient: HttpClient) {
  }

  private _user: IUser | null = null

  private userSignal = signal(this._user)

  public isAuthenticated = computed(() => this.userSignal() !== null)
  public getUserName = computed(() => this.userSignal()?.username);
  public getFullName = computed(() => this.userSignal()?.lastName + " " + this.userSignal()?.firstName);
  public getUserInfo = computed(() => this.userSignal())

  public loadUser() {
    const request = this.httpClient.get<IUser>("/user-info")
    return request.subscribe(value => {
      this.userSignal.set(value);
      console.log(`Load user: ${ value.username }`)
    })
  }

  public logout() {
    window.location.href = "/logout"
    effect(() => {
      this.userSignal()
    });
  }

  deleteAccount() {
    const request = this.httpClient.delete("/account")
      .pipe(
        catchError(this.handleAfterDelete)
      )
    return request.subscribe(value => console.log("Subscription res: ", value))
  }

  private handleAfterDelete(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      console.error('An error occurred:', error.error.message);
    }

    switch (error.status) {
      case 0: {
        window.location.href = "/account/login"
        return "need to login"
      }
      case 200: {
        window.location.href = error.url!
        return "ok"
      }
      default:
        console.error('An unexpected error occurred');
    }

    return throwError(error.statusText)
  }
}

interface IUser {
  id: string
  username: string
  email: string
  firstName: string
  lastName: string
  maxPlayersInList: number
  maxListsCount: number
}
