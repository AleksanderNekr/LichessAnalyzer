import { Component } from '@angular/core';
import { AuthService } from "../auth/auth.service";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  providers: [ AuthService ]
})
export class LoginComponent {
  constructor() {
  }
}
