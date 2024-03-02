import { computed, effect, Injectable, signal } from '@angular/core';
import { HttpClient } from "@angular/common/http";

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
