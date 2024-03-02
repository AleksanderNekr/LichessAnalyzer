import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { AuthService } from "../../auth/auth.service";

@Component({
  selector: 'app-login-menu',
  standalone: true,
  imports: [
    AsyncPipe,
    RouterLink
  ],
  templateUrl: './login-menu.component.html',
  styleUrl: './login-menu.component.css'
})
export class LoginMenuComponent implements OnInit {
  public isAuthenticated: boolean = false
  public userName: string | undefined

  constructor(private readonly authService: AuthService) {
  }

  ngOnInit(): void {
    this.isAuthenticated = this.authService.isAuthenticated()
    this.userName = this.authService.getUserName()
  }

  logout() {
    this.authService.logout()
  }
}
