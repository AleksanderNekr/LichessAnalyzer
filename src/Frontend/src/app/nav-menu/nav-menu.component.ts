import { Component } from '@angular/core';
import { LoginComponent } from "../login/login.component";
import { LoginMenuComponent } from "../login/login-menu/login-menu.component";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { NgClass } from "@angular/common";

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [
    LoginComponent,
    LoginMenuComponent,
    RouterLink,
    RouterLinkActive,
    NgClass
  ],
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.css'
})
export class NavMenuComponent {
  protected isExpanded: boolean = false

  public collapse() {
    this.isExpanded = false
  }

  public toggle() {
    this.isExpanded = !this.isExpanded
  }
}
