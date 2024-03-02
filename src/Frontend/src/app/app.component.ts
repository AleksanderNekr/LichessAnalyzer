import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavMenuComponent } from "./nav-menu/nav-menu.component";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from "./auth/auth.service";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ CommonModule, RouterOutlet, NavMenuComponent, NgbModule ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Frontend';

  constructor(private readonly authService: AuthService) {
  }

  ngOnInit(): void {
    this.authService.loadUser()
  }
}
