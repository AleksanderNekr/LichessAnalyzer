import { Component } from '@angular/core';
import { AuthService } from "../auth/auth.service";
import { NgOptimizedImage } from "@angular/common";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  providers: [ AuthService ]
})
export class LoginComponent {
  constructor() {
  }
}
