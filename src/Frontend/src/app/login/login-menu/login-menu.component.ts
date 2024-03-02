import { Component } from '@angular/core';
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
export class LoginMenuComponent {

  constructor(protected readonly authService: AuthService) {
  }

  logout() {
    this.authService.logout()
  }
}
