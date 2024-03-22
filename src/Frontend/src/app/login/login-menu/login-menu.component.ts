import { Component } from '@angular/core';
import { AsyncPipe } from "@angular/common";
import { RouterLink } from "@angular/router";
import { AuthService } from "../../auth/auth.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ModalDialogComponent } from "../../modals/modal-dialog/modal-dialog.component";
import { ButtonStyle } from "../../modals/modal-dialog/button.style";

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

  constructor(protected readonly authService: AuthService,
              private readonly modalService: NgbModal) {
  }

  logout() {
    let modal = this.modalService.open(ModalDialogComponent)
    modal.componentInstance.setSubmitCallback(() => this.authService.logout())
    modal.componentInstance.setTexts(
      "Confirm Exit",
      "Are you really want to exit from your account?",
      "Yes",
      "No"
    )
    modal.componentInstance.setSubmitBtnStyle(ButtonStyle.Danger)
  }
}
